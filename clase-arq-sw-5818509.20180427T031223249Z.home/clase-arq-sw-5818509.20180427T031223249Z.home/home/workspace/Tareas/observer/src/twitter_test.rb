# Observer Pattern
# Date: 26-Jan-2018
# Authors:
#          A01374527 Luis Daniel Rivero Sosa

#The source code contained in this file demonstrates how to
#run the test TwitterTest
require 'minitest/autorun'
require 'stringio'
require 'twitter'

#The Class TwitterTest test the Class Twitter with two diferent tests
class TwitterTest < Minitest::Test
  #Initialize the variables to run the tests
  def setup
    @out = StringIO.new
    @old_stdout = $stdout
    $stdout = @out
  end
  #Asign the old_stdout to stdout variable
  def teardown
    $stdout = @old_stdout
  end
  #Function that test the class Twitter
  def test_twitter_alices_adventures_in_wonderland
    
    a = Twitter.new('Alice')
    k = Twitter.new('King')
    q = Twitter.new('Queen')
    h = Twitter.new('Mad Hatter')
    c = Twitter.new('Cheshire Cat')
    
    a.follow(c)
    k.follow(q)
    
    h.follow(a).follow(q).follow(c)
    q.follow(q)

    a.tweet "What a strange world we live in."
    k.tweet "Begin at the beginning, and go on till you come "  \
      "to the end: then stop."
    q.tweet "Off with their heads!"
    c.tweet "We're all mad here."

    assert_equal \
      "Mad Hatter received a tweet from Alice: What a strange " \
        "world we live in.\n"                                   \
      "King received a tweet from Queen: Off with their "       \
        "heads!\n"                                              \
      "Mad Hatter received a tweet from Queen: Off with their " \
        "heads!\n"                                              \
      "Queen received a tweet from Queen: Off with their "      \
        "heads!\n"                                              \
      "Alice received a tweet from Cheshire Cat: We're all "    \
        "mad here.\n"                                           \
      "Mad Hatter received a tweet from Cheshire Cat: "         \
        "We're all mad here.\n",                                \
      @out.string
  end
  #Function that test the class Twitter
  def test_twitter_star_wars
    y = Twitter.new('Yoda')
    o = Twitter.new('Obi-Wan Kenobi')
    v = Twitter.new('Darth Vader')
    p = Twitter.new('Padmé Amidala')

    p.follow(v)
    v.follow(p).follow(y).follow(v)

    y.tweet "Do or do not. There is no try."
    o.tweet "The Force will be with you, always."
    v.tweet "I find your lack of faith disturbing."
    o.tweet "In my experience, there's no such thing as luck."
    y.tweet "Truly wonderful, the mind of a child is."
    p.tweet "I will not condone a course of action that will "  \
      "lead us to war."

    assert_equal \
      "Darth Vader received a tweet from Yoda: Do or do not. "  \
        "There is no try.\n"                                    \
      "Padmé Amidala received a tweet from Darth Vader: I find "\
        "your lack of faith disturbing.\n"                      \
      "Darth Vader received a tweet from Darth Vader: I find "  \
        "your lack of faith disturbing.\n"                      \
      "Darth Vader received a tweet from Yoda: Truly wonderful,"\
        " the mind of a child is.\n"                            \
      "Darth Vader received a tweet from Padmé Amidala: I will "\
        "not condone a course of action that will lead us to "  \
        "war.\n",                                               \
      @out.string
  end

end