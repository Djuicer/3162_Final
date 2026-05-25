extends Node2D

@export var mute: bool = false
@export var autoplay_music: bool = false

@onready var menu_music: AudioStreamPlayer2D = $MenuMusic
@onready var hover_button: AudioStreamPlayer2D = $ButtonHover
@onready var press_button: AudioStreamPlayer2D = $ButtonSelect
@onready var footstep: AudioStreamPlayer2D = $Footstep
@onready var game_music: AudioStreamPlayer2D = $GameMusic

var _footsteps_enabled: bool = false

func _ready():
	footstep.finished.connect(_on_footstep_finished)
	if autoplay_music and not mute:
		play_music()

func play_music():
	if not mute:
		if game_music.playing:
			game_music.stop()
		menu_music.play()

func stop_music():
	if menu_music.playing:
		menu_music.stop()
	if game_music.playing:
		game_music.stop()

func set_mute(value: bool):
	mute = value
	if mute:
		stop_music()
	else:
		play_music()
		
func play_hover_button():
	if not mute:
		hover_button.play()

func play_press_ui_button():
	if not mute:
		press_button.play()

func play_game_music():
	if not mute:
		if menu_music.playing:
			menu_music.stop()
		if not game_music.playing:
			game_music.play()

func stop_game_music():
	if game_music.playing:
		game_music.stop()

func start_footsteps():
	_footsteps_enabled = true
	if not mute and not footstep.playing:
		footstep.play()

func stop_footsteps():
	_footsteps_enabled = false
	if footstep.playing:
		footstep.stop()

func _on_footstep_finished():
	if _footsteps_enabled and not mute:
		footstep.play()
