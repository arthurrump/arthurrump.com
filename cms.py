import importlib
import re
import typer

from datetime import date
from jinja2 import Environment, FileSystemLoader
from os import path, scandir, makedirs
from sys import stderr

TEMPLATES = path.join(path.dirname(__file__), "templates")
FEED = path.join(path.dirname(__file__), "content", "feed")

app = typer.Typer()

@app.command()
def import_():
    pass

@app.command()
def new(template: str, slug: str, language: str = None, date: str = date.today().strftime("%Y%m%d")):
    # Check if there is already an item with this slug and fail if there is one
    re_name = re.compile(r"^\d{8}_" + slug + "(?:(?:.[a-z]{2})?.[a-z]+)?$")
    for entry in scandir(FEED):
        if re_name.match(entry.name):
            print(f"ERROR: An item with slug '{slug}' already exists.", file = stderr)
            return

    # Initialize jinja and get the template file
    jinja = Environment(loader = FileSystemLoader(TEMPLATES), autoescape = False)
    template_file = jinja.get_template(path.join(template, "index.md"))
    
    # Get the variables the init type (file or folder)
    get_spec = importlib.util.spec_from_file_location(template, path.join(TEMPLATES, template, "__init__.py"))
    get = importlib.util.module_from_spec(get_spec)
    get_spec.loader.exec_module(get)
    variables = get.get_variables()
    init_type = get.get_init_type()
    
    # Set the output path according to all options
    basename = f"{date}_{slug}"
    language_suffix = "." + language if language else ""
    if init_type == "folder":
        outpath = path.join(FEED, basename, "index" + language_suffix + ".md")
    else:
        outpath = path.join(FEED, basename + language_suffix + ".md")
    
    makedirs(path.dirname(outpath), exist_ok = True)
    with open(outpath, mode = "x") as outfile:
        outfile.write(template_file.render(variables))

if __name__ == "__main__":
    app()
