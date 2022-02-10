# CustomLearning

It's the most customizable downloadable keyboard touch typing trainer.

## Customizable?
CustomLearning allows change most application parameters such as
* Appearance
  * app's control colors
  * app's fonts
  * setup background image
  * image blur effect and parallax
  * beautiful animations, parallax, bumps, splashes
* Keyborad layouts
  * A lot of popular alternative layouts 
  * Create YOUR OWN layout
* Lessons
  * Single lessons
  * Special courses
  * Dictionaries
  * Glue a few lessons to a temporary course

![CustomLearning keyboard layout editor](https://sun9-30.userapi.com/impg/sQPxzeaaMVphOO87xckej1wDDXU5I0-mrpEscw/Q2gXKQCkWec.jpg?size=1171x640&quality=95&sign=7ff35ba2e503f4b034b93ddf701ac252&type=album)
![CustomLearning recent opened layouts list](https://sun9-11.userapi.com/impg/VjMRgA550WxI7Q5F2hUu-fnxfoVcU9TUrLTluA/dGNmj9MVh9M.jpg?size=1171x640&quality=95&sign=b9dbdbed20da4be42ea8909e76bb782f&type=album)

  ## What's inside?
  CustomLearning allows typers use these modes:
   1. Passing classic courses (1 by 1)
   2. Passing dictionaries (courses, where lesson chosen via random);
   3. Training in single lessons;
   4. Passing typing tests;
 
 Every course, lessons, dictionary and words for typing test can be changed by user :)

## App's UI interaction
There's 3 ways to open lessons, courses, layouts and skins in CustomLearning
 1. Writing commands via CLI  `Alt+C`
 2. Using dialogue windows
 3. Files drag-and-drop (preferenced)

And 2 ways to usual app using:
 1. Using mouse
 2. Using keyboard only via hotkeys


## App's shortcuts
 1. `Alt+M` To restart the lesson and to focus on input text box
 2. `Alt+C` To open the Command Line
 3. `Alt+D` To start demonstration typing mode (autotyper)
 4. `Alt+U` To open app update manager
 5. `Alt+W` To select 1 of 2 choosen layouts
 6. `Alt+R` To shuffle lesson's words via random
 7. `Alt+E` To open lesson editor
 8. `Alt+Shift+E` To open course editor

 `Alt` is more comfortable key to use, because the key is pressed by left thumb instead of left or right pinky.

## App skins - skin configurations
User can often change app appearance. To simplify this process CustomLearning allows user to use special files with app appearance parameters - skin configuration files.

![1st theme](https://sun9-27.userapi.com/impg/FnDIXusEZFtcHkMlUFJ5ZYPbNt2Uyq-RWQI0zA/JZIq0UFRq_E.jpg?size=1120x640&quality=95&sign=6a21a61f41f1bb9e3945cfdd3edba99c&type=album)
![2nd theme](https://sun9-24.userapi.com/impg/bwqTF27IlbLP749Trb4yK2KbDHeCZtqgNFM_zA/0Z8dts1N5hY.jpg?size=1120x640&quality=95&sign=7ce4e703a48b4fcc004677ff52134506&type=album)
![3rd theme](https://sun9-52.userapi.com/impg/40i19vkVqh-MAFRsNFqCU0_Co66yi3Z1Bax81w/S0KN_ZpBy84.jpg?size=1120x640&quality=95&sign=9fb0ce45f76e63a0d239efc6ba624d37&type=album)
![4th theme](https://sun9-63.userapi.com/impg/4Sa8ehvDYT8C97qggSSNbWu71d2toNPrj0DLqA/nLBzmth_I5Q.jpg?size=1120x640&quality=95&sign=c219014f802486c51067262c03a048f6&type=album)
![5th theme](https://sun9-38.userapi.com/impg/n0XWGt4tYLDfcFbtWA9PlEV4kzJWyduPbbv9bg/zToyEbYyEoY.jpg?size=1120x640&quality=95&sign=d0a3acd9432c3e4531450dd5904982fc&type=album)
![6th theme](https://sun9-40.userapi.com/impg/lVh96sik6xTCZNZTWN31FaGfqdaX5ZRoxcx66g/6f-w2eophg8.jpg?size=1120x640&quality=95&sign=e86fe96fc12573acdb8a5fd1f86f06d3&type=album)

There is 2 ways to export skins:
 1. Via CLI command and file explorer `cfg exp` 
 2. Via CLI command to copy config code to clipboard `cfg c`

And 3 ways to import skins:
 1. Via CLI command and file explorer `cfg imp`
 2. Via CLI command from clipboard `cfg p`
 3. Via file drap-and-drop

You can hold background images on the Internet, just insert image url into config file :)
Every skin cointains the code such as:
```
#config
#config

<<UserConfig:
    <<Fonts:
        <AppGUIFontFamily Imprint MT Shadow, 8.25pt, style=Bold>>
        <AppGUIFontColor #FFB0B0B0>>

        <LessonFontFamily Imprint MT Shadow, 8.25pt>>
        <LessonFontSize 36>>
        <LessonFontColor White>>
        <RaidedLessonFontColor #FF87431B>>

        <KeyboardFontFamily Segoe UI>>
        <KeyboardFontColor #FFDBDBDB>>
    :Fonts>>

    <<Opacity:
        <MenuOpacity 1>>
        <KeyboardOpacity 1>>
        <HandsOpacity 1>>
    :Opacity>>

    <<Hands:
        <ShowHands True>>
        <HandsColor #FFFF9D75>>
        <HandsThickness 6>>
    :Hands>>

    <<Keyboard:
        <KeyColor #88492a15>>
        <BorderColor #341f07>>
        <HighlightColor #FFD65910>>
        <MistakeColor #FFE8B817>>
    :Keyboard>>

    <<AppColors:
        <MainColor #3f1d0b>>
        <SecondaryColor #FF381309>>
        <TextBoxColor #FF1B0D09>>
        <SecondaryMenuColor #0c0402>>
        <Theme dark>>
        <ControlsHighlightColor brown>>
        <HasImageBackground True>>
        <InputTextBox SingleLineWithStaticCaret>>
    :AppColors>>

    <<Wallpaper:
        <PathToImage https://images3.alphacoders.com/210/210895.jpg>>
        <BlurRadius 31.3043478260869>>
    :Wallpaper>>

    <<Animations:
        <EnableParallaxEffect True>>
        <EnabledSplash True>>
        <ChosenKeyboardShape ClassicDonut>>
        <ChosenMouseShape NiceLightingDonut>>
        <EnableClickBump True>>
        <EnableTypeBump False>>
    :Animations>>
:UserConfig>>
```
  

## Buy me a coffee :)
Ethereum: 0xdbA4C1467644b706CB64621C874297Ba03005743
