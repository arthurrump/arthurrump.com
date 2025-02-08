"""
Processes `aliases` metadata and writes a _redirect file for use with Netlify or
Cloudflare Pages
"""

from os import path
from pelican import Pelican, signals
from pelican.generators import Generator, ArticlesGenerator, PagesGenerator

def init(pelican: Pelican):
    global output_path
    output_path = pelican.output_path

def _write_aliases(metadata, url, redirects):
    for alias in metadata.get("aliases", []):
        redirects.write(alias)
        redirects.write(" /")
        redirects.write(url)
        redirects.write(" 301\n")

def create_redirects(generators: list[Generator]):
    with open(path.join(output_path, "_redirects"), "w") as redirects:
        for gen in generators:
            if isinstance(gen, ArticlesGenerator):
                for article in gen.articles:
                    _write_aliases(article.metadata, article.url, redirects)
            elif isinstance(gen, PagesGenerator):
                for page in gen.pages:
                    _write_aliases(page.metadata, page.url, redirects)

def register():
    signals.initialized.connect(init)
    signals.all_generators_finalized.connect(create_redirects)
