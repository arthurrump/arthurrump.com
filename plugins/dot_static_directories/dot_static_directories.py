"""
Find all folders within the content folder that contain a .static file and add
those to the STATIC_PATHS list.
"""

from os import path
from glob import glob
from pelican import Pelican, signals

def init(pelican: Pelican):
    static_paths = [
        path.dirname(dot_static)
        for dot_static in glob("**/.static", root_dir=pelican.settings["PATH"], recursive=True)
    ]
    pelican.settings["STATIC_PATHS"].extend(static_paths)
    pelican.settings["ARTICLE_EXCLUDES"].extend(static_paths)
    pelican.settings["PAGE_EXCLUDES"].extend(static_paths)
    print(pelican.settings["STATIC_PATHS"])

def register():
    signals.initialized.connect(init)
