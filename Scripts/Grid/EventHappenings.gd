extends Resource
class_name EventHappenings

var event_list = []
var event_lookup: Dictionary
var default_game_settings: Dictionary
var current_index = 0

func _init() -> void:
    event_list = GameData.itemData["events"]
    default_game_settings = GameData.itemData["game settings"].duplicate()

    # Sort the event list by the time they happen, to make doing and undoing events easier
    event_list.sort_custom(Callable(self, "_sort_events"))
    event_lookup = {
        "drought": Callable(self, "_drought"),
        "rainstorm": Callable(self, "_rainstorm"),
        "normal": Callable(self, "_normal")
    }

func _sort_events(a, b) -> bool:
    assert("time" in a, "Event does not have a time field")
    assert("time" in b, "Event does not have a time field")
    return a["time"] < b["time"]

func check_events(time: int):
    if current_index >= event_list.size():
        return
    var event = event_list[current_index]
    if event["time"] == time:
        if event["type"] in event_lookup:
            event_lookup[event["type"]].call()
        else :
            print("Event type not found: ", event["type"])
            event_lookup["normal"].call()
        current_index += 1

func check_undo_events(time: int):
    # If no events have happened yet, do nothing
    if current_index == 0:
        return
    # If only one event has happened, and it was supposed to happen at this time, undo it by returning to the default state
    if current_index == 1:
        if event_list[0]["time"] - 1 == time:
            event_lookup["normal"].call()
            current_index -= 1
        return
    
    var event = event_list[current_index - 1]
    var previousEvent = event_list[current_index - 2]
    # If the current event was supposed to happen at this time, undo it by calling the previous event
    if event["time"] - 1 == time:
        if previousEvent["type"] in event_lookup:
            event_lookup[previousEvent["type"]].call()
        else :
            print("Event type not found: ", event["type"])
            event_lookup["normal"].call()
        current_index -= 1

func _drought():
    print("Drought event")
    pass

func _rainstorm():
    print("Rainstorm event")
    pass

func _normal():
    print("Normal event")
    pass