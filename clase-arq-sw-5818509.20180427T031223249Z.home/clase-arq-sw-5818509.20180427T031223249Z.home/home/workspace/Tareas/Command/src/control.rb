# Command Pattern
# Date: 08-Feb-2018
# Author:
#          A01374527 Luis Daniel Rivero Sosa 

# The source code contained in this file demonstrates how to
# implement the <em>command pattern</em>.

# A class that models a Remote Control With Undo, based in the command pattern

class RemoteControlWithUndo
#Method that initialize the variables
  def initialize
    @on_commands = []
    @off_commands = []
    no_command = NoCommand.new
    7.times do
      @on_commands << no_command
      @off_commands << no_command
    end
    @undo_command = no_command
  end
#Method that set a command in a slat
  def set_command(slot, on_command, off_command)
    @on_commands[slot] = on_command
    @off_commands[slot] = off_command
  end
#Method that simulate that a button on was pushed
  def on_button_was_pushed(slot)
    @on_commands[slot].execute
    @undo_command = @on_commands[slot]
  end
#Method that simulate that a button off was pushed
  def off_button_was_pushed(slot)
    @off_commands[slot].execute
    @undo_command = @off_commands[slot]
  end
#Method that simulate that a undo on was pushed
  def undo_button_was_pushed()
    @undo_command.undo
  end
#Method that inspect the reote control status
  def inspect
    string_buff = ["\n------ Remote Control -------\n"]
    @on_commands.zip(@off_commands) \
        .each_with_index do |commands, i|
      on_command, off_command = commands
      string_buff << \
      "[slot #{i}] #{on_command.class}  " \
        "#{off_command.class}\n"
    end
    string_buff << "[undo] #{@undo_command.class}\n"
    string_buff.join
  end

end
#Class that define a no command
class NoCommand
#Method of command execution
  def execute
  end
#Method of undo command
  def undo
  end

end
#Class that simulate the light behaviour 
class Light
#Level atribute to read
  attr_reader :level
#Method that initialize the variables
  def initialize(location)
    @location = location
    @level = 0
  end
#Method that siulate the press of button "on"
  def on
    @level = 100
    puts "Light is on"
  end
#Method that siulate the press of button "of"
  def off
    @level = 0
    puts "Light is off"
  end
#Method that return the status of light
  def dim(level)
    @level = level
    if level == 0
      off
    else
      puts "Light is dimmed to #{@level}%"
    end
  end

end
#Class that simulate the Ceiling fan behaviour 
class CeilingFan

  # High speed
  HIGH   = 3
  #Medium speed
  MEDIUM = 2
  #Low speed
  LOW    = 1
  #no Speed
  OFF    = 0
#Speed atribute to read
  attr_reader :speed
#Method that initialize the variables
  def initialize (location)
    @location = location
    @speed = OFF
  end
#Method that simulate a high speed
  def high
    @speed = HIGH
    puts "#{@location} ceiling fan is on high"
  end
#Method that simulate a medium speed
  def medium
    @speed = MEDIUM
    puts "#{@location} ceiling fan is on medium"
  end
#Method that simulate a low speed
  def low
    @speed = LOW
    puts "#{@location} ceiling fan is on low"
  end
#Method that simulate empty speed
  def off
    @speed = OFF
    puts "#{@location} ceiling fan is off"
  end

end
#class that represents Light On Command
class LightOnCommand
    #Method that initialize the variables
    def initialize(lightO)
      @light = lightO
        
    end
    #Method of command execution
    def execute
        @light.on
    end
#Method of undo command
    def undo
        @light.off
    end
end
#class that represents light off command
class LightOffCommand
  #Method that initialize the variables
    def initialize(lightO)
      @light = lightO
        
    end
    #Method of command execution
    def execute
        @light.off
    end
#Method of undo command
    def undo
        @light.on
    end
end
#class that represents ceiling fan high command
class CeilingFanHighCommand 
  #Method that initialize the variables
    def initialize(fan)
        @fan = fan
        @status = fan.speed
    end
    #Method of command execution
    def execute
        @status = @fan.speed
        @fan.high
    end
#Method of undo command
    def undo
        case @status
        when CeilingFan::HIGH
          @fan.high
        when CeilingFan::MEDIUM
          @fan.medium
        when CeilingFan::OFF
          @fan.off
        when CeilingFan::LOW
          @fan.low
        end
        @status = @fan.speed
    end
end
#class that represents ceiling medium command
class CeilingFanMediumCommand 
  #Method that initialize the variables
    def initialize(fan)
        @fan = fan
        @status = fan.speed
    end
    #Method of command execution
    def execute
        @status = @fan.speed
        @fan.medium
    end
#Method of undo command
    def undo
        case @status
        when CeilingFan::HIGH
          @fan.high
        when CeilingFan::MEDIUM
          @fan.medium
        when CeilingFan::OFF
          @fan.off
        when CeilingFan::LOW
          @fan.low
        end
        @status = @fan.speed
    end
end
#class that represents ceiling fan command
class CeilingFanOffCommand 
  #Method that initialize the variables
    def initialize(fan)
        @fan = fan
        @status = fan.speed
    end
    #Method of command execution
    def execute
        @status = @fan.speed
        @fan.off
        
    end
#Method of undo command
    def undo
        case @status
        when CeilingFan::HIGH
          @fan.high
        when CeilingFan::MEDIUM
          @fan.medium
        when CeilingFan::OFF
          @fan.off
        when CeilingFan::LOW
          @fan.low
        end
        @status = @fan.speed
        
    end
end
