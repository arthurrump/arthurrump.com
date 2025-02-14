import importlib
ask = importlib.import_module("..templates", ".")

def get_init_type():
    return "folder"

def get_variables():
    return {
        "title": ask.ask("title"),
        "type_for_venue": ask.ask("type for venue", "Conference paper for Some Conf 2022"),
        "authors": ask.ask_list("authors"),
        "tags": ask.ask_list("tags"),
        "abstract": ask.ask("abstract", default = "<!-- Place the abstract here. -->"),
        "doi": ask.ask("doi")
    }
