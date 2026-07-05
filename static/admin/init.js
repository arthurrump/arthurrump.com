window.CMS_MANUAL_INIT = true;

(function () {
  const hashtagPattern = /(^|[^\p{L}\p{N}_/])#([\p{L}\p{N}][\p{L}\p{N}_-]*)/gu;

  function spacifyTag(tag) {
    return tag
      .replace(/_/g, ' ')
      .replace(/([\p{Ll}\p{Nd}])([\p{Lu}])/gu, '$1 $2')
      .replace(/([\p{Lu}]+)([\p{Lu}][\p{Ll}])/gu, '$1 $2')
      .trim();
  }

  function extractTags(body) {
    const tags = [];

    for (const match of body.matchAll(hashtagPattern)) {
      const tag = spacifyTag(match[2]);
      if (tag && !tags.includes(tag)) {
        tags.push(tag);
      }
    }

    return tags;
  }

  function setTags(data) {
    const body = data.get('body');
    if (typeof body !== 'string') {
      return data;
    }

    return data.set('tags', extractTags(body));
  }

  CMS.registerEventListener({
    name: 'preSave',
    handler: ({ entry }) => {
      if (entry.get('collection') !== 'notes') {
        return entry;
      }

      let nextEntry = entry.set('data', setTags(entry.get('data')));
      const localized = entry.get('i18n');

      if (localized) {
        localized.forEach((localeData, locale) => {
          nextEntry = nextEntry.setIn(['i18n', locale, 'data'], setTags(localeData.get('data')));
        });
      }

      return nextEntry;
    },
  });

  CMS.init();
})();
