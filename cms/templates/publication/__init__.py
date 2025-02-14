from cms.ask import ask, ask_list

def get_init_type():
    return "folder"

def get_variables():
    return {
        "title": ask("title"),
        "type_for_venue": ask("type for venue", "Conference paper for Some Conf 2022"),
        "authors": ask_list("authors"),
        "tags": ask_list("tags"),
        "abstract": ask("abstract", default = "<!-- Place the abstract here. -->"),
        "doi": ask("doi")
    }
