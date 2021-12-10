# CustomLearning

It's the most customizable downloadable keyboard touch typing trainer.

## Customizable?
CustomLearning allows change most application parameters such as
* Appearance
  * app's control colors
  * app's fonts
  * setup background image
  * image blur effect and parallax
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

![1st theme](https://sun9-81.userapi.com/impg/y2vdK3pfVKPMJJFvREBiVuwpWix2Z4OKI_JcUg/gxBZ2QfiWlY.jpg?size=1193x640&quality=95&sign=a21a524491fbf55fba7c6e361526c8c6&type=album)
![2nd theme](https://sun9-77.userapi.com/impg/Puwp6zEXOzbK2xldz5eqAgl-Wt5nj5X4PNPvNw/GZKwG4rQAx0.jpg?size=1193x640&quality=95&sign=6012b79fb7e12e267567ff174fb51935&type=album)
![3rd theme](https://sun9-71.userapi.com/impg/CVhvHznjUI5ekGsEe-CPl03fTkkDfkXOug7ugg/KlAkh5E4Lks.jpg?size=1193x640&quality=95&sign=cd7fe24eecd32680d124f04d7f0146ec&type=album)

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
<<UserConfig:
    <<Fonts:
        <AppGUIFontFamily Comic Sans MS>>
        <AppGUIFontColor White>>
        <LessonFontFamily Comic Sans MS>>
        <LessonFontSize 36>>
        <LessonFontColor #FF65FF0D>>
        <RaidedLessonFontColor #FF2D6B09>>
        <KeyboardFontFamily Comic Sans MS>>
        <KeyboardFontColor #FFDBDBDB>>
    :Fonts>>

     <<Opacity:
         <MenuOpacity 0.308695652173913>>
         <KeyboardOpacity 1>>
         <HandsOpacity 1>>
     :Opacity>>

     <<Hands:
         <ShowHands True>>
         <HandsColor #FF02E051>>
         <HandsThickness  6>>
    :Hands>>

    <<Keyboard:
        <KeyColor #6603680E>>
        <BorderColor #CA023B08>>
        <HighlightColor #FF00FF1B>>
        <MistakeColor #FFFF9300>>
    :Keyboard>>
  
    <<AppColors:
        <MainColor #FF4BA11A>>
        <SecondaryColor #FF1D4505>>
        <TextBoxColor #FF183805>>
        <SecondaryMenuColor #FF378708>>
        <Theme dark>>
        <ControlsHighlightColor green>>
        <HasImageBackground True>>
        <InputTextBox SingleLineWithStaticCaret>>
    :AppColors>>

    <<Wallpaper:
        <PathToImage D:\Storage\Photos\Wallpapers\5210941.jpg>>
        <HasParallaxEffect True>>
        <BlurRadius 100>>
    :Wallpaper>>
:UserConfig>>
```
  

## Buy me a coffee :)
Ethereum: 0xdbA4C1467644b706CB64621C874297Ba03005743
