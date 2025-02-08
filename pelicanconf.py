import os

# Basic information
SITENAME = "Arthur Rump"
AUTHOR = "Arthur Rump"

MENU_PAGES = [ "about" ]
MENU_CATEGORIES = [ "posts", "publications" ]

LINKS = ( # Links in the header
    ("Pelican", "https://getpelican.com/"),
    ("Python.org", "https://www.python.org/"),
    ("Jinja2", "https://palletsprojects.com/p/jinja/"),
    ("You can modify those links in your config file", "#"),
)

SOCIAL = ( # Links in the "social" section
    ("You can add links in your config file", "#"),
    ("Another social link", "#"),
)

# Theme, plugins, layout
THEME = "./theme"

PLUGIN_PATHS = [ "./plugins", os.environ["PELICAN_PLUGINS"] ]
PLUGINS = [ "pandoc_reader" ]

PATH = "content"
STATIC_PATHS = [ ]

SLUGIFY_SOURCE = "basename" # should only be used by pages, with posts matching the formats below
FILENAME_METADATA = r"(?P<date>\d{8})_(?P<slug>[^.]*)(\.(?P<lang>[a-z]{2}))?"
PATH_METADATA = r"(?P<date>\d{8})_(?P<slug>[^.]*)/index(\.(?P<lang>[a-z]{2}))?\."

USE_FOLDER_AS_CATEGORY = False
DEFAULT_CATEGORY = "" # Force to specify a category

TYPOGRIFY = True
TYPOGRIFY_OMIT_FILTERS = [ "amp", "caps", "initial_quotes" ]

SUMMARY_MAX_LENGTH = 50
SUMMARY_MAX_PARAGRAPHS = 2

# Localization
TIMEZONE = "Europe/Amsterdam"
DEFAULT_DATE_FORMAT = "%Y-%m-%d"

DEFAULT_LANG = "en"

# URLs
ARTICLE_URL = "{slug}/"
ARTICLE_SAVE_AS = "{slug}/index.html"
ARTICLE_LANG_URL = "{lang}/{slug}/"
ARTICLE_LANG_SAVE_AS = "{lang}/{slug}/index.html"

DRAFT_URL = "drafts/{slug}/"
DRAFT_SAVE_AS = "drafts/{slug}/index.html"
DRAFT_LANG_URL = "{lang}/drafts/{slug}/"
DRAFT_LANG_SAVE_AS = "{lang}/drafts/{slug}/index.html"

PAGE_URL = "{slug}/"
PAGE_SAVE_AS = "{slug}/index.html"
PAGE_LANG_URL = "{lang}/{slug}/"
PAGE_LANG_SAVE_AS = "{lang}/{slug}/index.html"

DRAFT_PAGE_URL = "drafts/{slug}/"
DRAFT_PAGE_SAVE_AS = "drafts/{slug}/index.html"
DRAFT_PAGE_LANG_URL = "{lang}/drafts/{slug}/"
DRAFT_PAGE_LANG_SAVE_AS = "{lang}/drafts/{slug}/index.html"

CATEGORY_URL = "{slug}/"
CATEGORY_SAVE_AS = "{slug}/index.html"
TAG_URL = "tags/{slug}/"
TAG_SAVE_AS = "tags/{slug}/index.html"
AUTHOR_URL = "authors/{slug}/"
AUTHOR_SAVE_AS = "authors/{slug}/index.html"

ARCHIVES_URL = "archives/"
ARCHIVES_SAVE_AS = "archives/index.html"
AUTHORS_URL = "authors/"
AUTHORS_SAVE_AS = "authors/index.html"
TAGS_URL = "tags/"
TAGS_SAVE_AS = "tags/index.html"
CATEGORIES_URL = "categories/"
CATEGORIES_SAVE_AS = "categories/index.html"

YEAR_ARCHIVE_URL = "archives/{date:%Y}/"
YEAR_ARCHIVE_SAVE_AS = "archives/{date:%Y}/index.html"

DEFAULT_PAGINATION = 10
DEFAULT_ORPHANS = 2
PAGINATION_PATTERNS = [
    (1, "{base_name}/", "{save_as}"),
    (2, "{base_name}/page/{number}/", "{base_name}/page/{number}/index{extension}")
]

# Feed generation
FEED_ATOM = "feed.atom.xml"
FEED_RSS = "feed.xml"

TRANSLATION_FEED_ATOM = "{lang}/feed.atom.xml"
TRANSLATION_FEED_RSS = "{lang}/feed.xml"

CATEGORY_FEED_ATOM = "{slug}/feed.atom.xml"
CATEGORY_FEED_RSS = "{slug}/feed.xml"

AUTHOR_FEED_ATOM = None
AUTHOR_FEED_RSS = None

FEED_MAX_ITEMS = None

# Generation settings
DELETE_OUTPUT_DIRECTORY = True
