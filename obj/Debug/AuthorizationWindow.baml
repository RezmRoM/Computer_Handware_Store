   M S B A M L   `   `   `  ����S  OComputer_Hardware_Strore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Computer_Hardware_StroreComputer_Hardware_Strore  5   	   2    ,Computer_Hardware_Strore.AuthorizationWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   8local&clr-namespace:Computer_Hardware_Strore     5   	     � Title$F  @Авторизация - Компьютерный Магазин��5	   	   $	��750��5
   	   $
��1200��5   	    � WindowStartupLocation$ CenterScreen=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency . 
������  � 
ResizeMode$ CanResizeWithGrip=�5   	    � 	Resources 5         PrimaryColor  SecondaryColor  BackgroundColor  SurfaceColor  	TextColor  AccentColor  PrimaryBrush �� Color  SecondaryBrush  BackgroundBrush 	 SurfaceBrush 
 	TextBrush  AccentBrush  ModernButton �� 
TargetType �� Property	 �� Value
 �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value N� EnterActions �� TargetProperty	 �� To g� Duration N� ExitActions  ModernTextBox ��Name  ModernPasswordBox��%�	  &	        &	      &	 >     &	 ]     &	 |     &	 �     &	 �     7   &	 �     7  &	 �     7  &		     7  &	
     7  &	 2    7  &	 J    7  7 
 7  7  7   &	 �    7 	 7 
 7  7  &	 �    7 	 7 
 7  7  ��5   
   #FF3366��6.   ��5   
   #2563EB��60   ��5   
   #1A1A1A��61   ��5   
   #2D2D2D��6.   ��5   
   #FFFFFF��6+   ��5   
   #00FF9D��6-   �� 5   
   8   6/   �� 5   
   8   61   �� 5   
   8   62   �� 5   
   8   6/   �� 5   
   8   6,   �� 5    
   8   6.   �� 5#   
   " ��6%   .�5$      ��  � ��8	   6+   �� 5%       � ��8	  6+   �� 5&       � ��$	 0q�60   �� 5'       � ��$	 20,10q�6(   �� 5(       �@�Cursor$
	 Hand��6'   �� 5)       � ��	 5*      �� 5+      "
 ��6&   .R�5,      �� #��z��6!   $ 8��5-   !   #��z��5.   !   #��z��5/   !   ._�50      �� $��Center=�6/   $��Center=�51   -   52      53      55       56      P� 57       �@J�IsMouseOver . . �58      ��  � ��8	  63    59      �� 5:      .��5;      �� .��5<   "   �� $1 +(Button.Background).(SolidColorBrush.Color)�61   8  $ 0:0:0.2D�5>   /   5?      5@       5B      �� 5C      .��5D      �� .��5E   "   �� $1 +(Button.Background).(SolidColorBrush.Color)�61   8  $ 0:0:0.2D�5G   /   5H      5I      5K      5M      �� 5P   
   " ��6&   .�5Q      ��  � ��8	   6+   �� 5R       � ��8	  6+   �� 5S       � ��8	  6,   �� 5T       � ��$	 2q�60   �� 5U       � ��$
	 10,5q�6(   �� 5V       � ��$	 Center=�69   �� 5W       � ��	 5X      �� 5Y      "
 ��6&   .R�5Z      �� #��z��6!   #��z��5[   !   #��z��5\   !   $ 8��5]   !   ._�5^      �� $ PART_ContentHost��6+   5_      5`      5b       5c      P� 5d       �@J�	IsFocused . . �5e      ��  � ��8	  64   5f      5h      �� 5k   
   " G�6*   .�5l      ��  � ��8	   6+   �� 5m       � ��8	  6+   �� 5n       � ��8	  6,   �� 5o       � ��$	 2q�60   �� 5p       � ��$
	 10,5q�6(   �� 5q       � ��$	 Center=�69   �� 5r       � ��	 5s      �� 5t      "
 G�6&   .R�5u      �� #��z��6!   #��z��5v   !   #��z��5w   !   $ 8��5x   !   ._�5y      �� $ PART_ContentHost��6+   5z      5{      5}       5~      P� 5       �@J�	IsFocused . . �5�      ��  � ��8	  64   5�      5�      5�      .��5�      �� #��[ 6   $ 20��5�      $��1q�5�      #��[ 5�      ._�5�   
   �  � RowDefinitions 5�      �� 5�      $��30 �6    �� 5�      $��* �6    .+�5�      � -   5        $��0��5�      
������ .+�5�      �� $F��@Компьютерный Магазин - Авторизация��6   #��[
 5�      $��10,0,0,0q�5�      $��Center=�5�      �� 5�       �� Orientation$ 
Horizontal=�6   $��Right=�5�      $��0,0,5,0q�5�      .+�5�      �� -   5         ��Name$ MinimizeButton��5�      $	��—��5�      #��[ 5�      $��30��5�      $��25��5�      �� 5�      -   5        $ MaximizeButton��5�      $	��□��5�      #��[ 5�      $��30��5�      $��25��5�      $	��5,0q�5�      �� 5�      -   5        $ CloseButton��5�      $	��✕��5�      #��[ 5�      $��30��5�      $��25��5�      5�      5�      � 5�      $��1��6    � ColumnDefinitions 5�      �� 5�      $��* �6'   �� 5�      $	��400 �6'   .+�5�      � $��0��6   .+�5�      �� $!��/Images/tech_background.jpg��6   $��UniformToFill=�5�       J� Effect 5�      -  'System.Windows.Media.Effects.BlurEffect  5�         Radius$ 5Q�6)   5�      �� 5�      -   5         ��Name$ AnimatedGrid��5�      }�. 5�      5�      �� 5�      $��1��6   #��[	 5�      $ 	20,0,0,20��5�      $��0q�5�      ._�5�      �� $��40q�6!   $��Center=�5�   !   .+�5�      �� $��Авторизация��6$   $��32(�5�   $   $
��Bold"�5�   $   #��[
 5�   $   $��0,0,0,40q�5�   $   �� 5�      $��Email��6$   #��[
 5�   $   $��0,0,0,5q�5�   $   �� 5�      -   5         ��Name$ EmailTextBox��5�   "   $��40��5�   "   $��16(�5�   "   #��[ 5�   "   �� 5�      $��Пароль��6$   #��[
 5�   $   $��0,20,0,5q�5�   $   G� 5�      -   5         G�Name$ PasswordBox��5�   &   $��40��5�   &   $��16(�5�   &   #��[ 5�   &   �� 5�      -   5        $��
Войти��5�   !   #��[ 5�   !   $��0,40,0,0q�5�   !   $��45��5�   !   $��16(�5�   !   5�   !   �� 5�      $��0,20,0,0q�6$   $��Center=�5�   $   �� 5�      -	   5         ��Name$ RegisterLink��5�   (   #��[ 5�   (    �� TextDecorations$
 None{�5�   (   .�5�   C   .�5�   "   �� $*��$Зарегистрироваться��6,   $��14(�5�   ,   5�      5�      5�      5�      5�      5�      5�      5�      