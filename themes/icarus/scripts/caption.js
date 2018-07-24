hexo.extend.filter.register('after_post_render', function (data) {
    if (data.layout == 'post' || data.layout == 'page' || data.layout == 'about') {
        data.content = data.content.replace(/(<img [^>]*title="([^"]+)"[^>]*>)/g, '$1' + '<span class="caption">$2</span>');
    }
    return data;
});