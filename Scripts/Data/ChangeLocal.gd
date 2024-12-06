extends Control

func change_local(language_code: String):
	TranslationServer.set_locale(language_code)
