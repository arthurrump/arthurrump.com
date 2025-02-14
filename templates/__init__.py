def ask(thing: str, placeholder: str = None, default: str = "") -> str:
    result = input(thing + (f" (eg. '{placeholder}')" if placeholder else "") + ": ")
    if result == "":
        return default
    return result

def ask_list(thing: str, placeholder: str = None) -> list[str]:
    results = []
    i = 0
    result = input(f"{thing}[{i}]" + (f" (eg. '{placeholder}')" if placeholder else "") + ": ")
    while result != "":
        results.append(result)
        i += 1
        result = input(f"{thing}[{i}]: ")
    return results

def ask_init_type(default = None):
    result = input("initialize as file or folder: ").lower()
    while result not in [ "file", "folder" ] and not (default and result == ""):
        result = input("choose file or folder: ").lower()
    return result if result != "" else default
