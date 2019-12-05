# Command Pattern
# Date: 08-Feb-2018
# Author:
#          A01374527 Luis Daniel Rivero Sosa 

# The source code contained in this file demonstrates how to
# implement the <em>command pattern test</em>.

require 'minitest/autorun'
require 'stringio'
require 'control'

#Class that simulate the test of RemoteControlWithUndo

class ControlTest < Minitest::Test
#Method that initialize the variables
  def setup
    set_stdout
    @rc = RemoteControlWithUndo.new
    set_light
    set_fan
  end
#method that call reset stdout method
  def teardown
    reset_stdout
  end
#Method that set the stdout
  def set_stdout
    @out = StringIO.new
    @old_stdout = $stdout
    $stdout = @out
  end
#Method that reset stdout
  def reset_stdout
    $stdout = @old_stdout
  end
#method that initialize the light variables to test
  def set_light
    light = Light.new("Living Room")
    light_on = LightOnCommand.new(light)
    light_off = LightOffCommand.new(light)
    @rc.set_command(0, light_on, light_off)
  end
#method that initialize the fan variables to test
  def set_fan
    fan = CeilingFan.new("Living Room")
    fan_medium = CeilingFanMediumCommand.new(fan)
    fan_high = CeilingFanHighCommand.new(fan)
    fan_off = CeilingFanOffCommand.new(fan)
    @rc.set_command(1, fan_medium, fan_off)
    @rc.set_command(2, fan_high, fan_off)
  end
#Method that test light buttons
  def test_light
    @rc.on_button_was_pushed(0)
    @rc.off_button_was_pushed(0)
    p @rc
    @rc.undo_button_was_pushed
    @rc.off_button_was_pushed(0)
    @rc.on_button_was_pushed(0)
    p @rc
    @rc.undo_button_was_pushed
    assert_equal                                                \
      "Light is on\n"                                           \
      "Light is off\n"                                          \
      "\n------ Remote Control -------\n"                       \
      "[slot 0] LightOnCommand  LightOffCommand\n"              \
      "[slot 1] CeilingFanMediumCommand  CeilingFanOffCommand\n"\
      "[slot 2] CeilingFanHighCommand  CeilingFanOffCommand\n"  \
      "[slot 3] NoCommand  NoCommand\n"                         \
      "[slot 4] NoCommand  NoCommand\n"                         \
      "[slot 5] NoCommand  NoCommand\n"                         \
      "[slot 6] NoCommand  NoCommand\n"                         \
      "[undo] LightOffCommand\n\n"                              \
      "Light is on\n"                                           \
      "Light is off\n"                                          \
      "Light is on\n"                                           \
      "\n------ Remote Control -------\n"                       \
      "[slot 0] LightOnCommand  LightOffCommand\n"              \
      "[slot 1] CeilingFanMediumCommand  CeilingFanOffCommand\n"\
      "[slot 2] CeilingFanHighCommand  CeilingFanOffCommand\n"  \
      "[slot 3] NoCommand  NoCommand\n"                         \
      "[slot 4] NoCommand  NoCommand\n"                         \
      "[slot 5] NoCommand  NoCommand\n"                         \
      "[slot 6] NoCommand  NoCommand\n"                         \
      "[undo] LightOnCommand\n\n"                               \
      "Light is off\n", @out.string
  end
#Method that test fan buttons
  def test_fan
    @rc.on_button_was_pushed(1)
    @rc.off_button_was_pushed(1)
    p @rc
    @rc.undo_button_was_pushed
    @rc.on_button_was_pushed(2)
    p @rc
    @rc.undo_button_was_pushed
    assert_equal                                                \
      "Living Room ceiling fan is on medium\n"                  \
      "Living Room ceiling fan is off\n"                        \
      "\n------ Remote Control -------\n"                       \
      "[slot 0] LightOnCommand  LightOffCommand\n"              \
      "[slot 1] CeilingFanMediumCommand  CeilingFanOffCommand\n"\
      "[slot 2] CeilingFanHighCommand  CeilingFanOffCommand\n"  \
      "[slot 3] NoCommand  NoCommand\n"                         \
      "[slot 4] NoCommand  NoCommand\n"                         \
      "[slot 5] NoCommand  NoCommand\n"                         \
      "[slot 6] NoCommand  NoCommand\n"                         \
      "[undo] CeilingFanOffCommand\n\n"                         \
      "Living Room ceiling fan is on medium\n"                  \
      "Living Room ceiling fan is on high\n"                    \
      "\n------ Remote Control -------\n"                       \
      "[slot 0] LightOnCommand  LightOffCommand\n"              \
      "[slot 1] CeilingFanMediumCommand  CeilingFanOffCommand\n"\
      "[slot 2] CeilingFanHighCommand  CeilingFanOffCommand\n"  \
      "[slot 3] NoCommand  NoCommand\n"                         \
      "[slot 4] NoCommand  NoCommand\n"                         \
      "[slot 5] NoCommand  NoCommand\n"                         \
      "[slot 6] NoCommand  NoCommand\n"                         \
      "[undo] CeilingFanHighCommand\n\n"                        \
      "Living Room ceiling fan is on medium\n", @out.string
  end

end