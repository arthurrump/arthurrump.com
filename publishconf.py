# Settings specific for publication.

import os
import sys

sys.path.append(os.curdir)
from pelicanconf import *

# If your site is available via HTTPS, make sure SITEURL begins with https://
SITEURL = "https://arthurrump.com"
FEED_DOMAIN = SITEURL
RELATIVE_URLS = False

DELETE_OUTPUT_DIRECTORY = True
