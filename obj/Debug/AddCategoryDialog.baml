   M S B A M L   `   `   `  ����  Computer_Hardware_Strore0    *Computer_Hardware_Strore.AddCategoryDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$-  'Добавление категории��5   	   $	��284��5   	   $	��400��5   	    � WindowStartupLocation$ CenterOwner=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency . 
������ .��5      ��    SurfaceBrush#��[  6    �� CornerRadius$ 15��5      $��
10,10,0,10q�5      $
��Left=�6!   $	��390��6<    J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5         
BlurRadius$ 10Q�6      ShadowDepth$ 0Q�5         Color$ #2563EB��5      ._�5   
   � $��30,30,17,30q�6   	 � RowDefinitions	 5      �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    .+�5      �� $8��2Добавление новой категории��6   $��24(�5      $��SemiBold"�5        	TextBrush#��[ 5      $��Center=�5       $��0,0,0,20q�5!      �� 5#      $��1��6   $
��0,10q�6&   .+�5$      �� $*��$Название категории:��6   #��[ 5%      $��0,0,0,5q�5&      �� 5'      -   5        
 ��Name$
 CategoryNameTextBox��5'        ModernTextBox#��[ 5(      $��40��5)      $��0,0,0,15q�5*      5+      �� 5-      $��2��6    �� Orientation$ 
Horizontal=�5.      $��Center=�5/      $��0,20,0,0q�50      .+�51      �� -   5        $��Добавить��51        ModernButton#��[ 52      $	��120��53      $��40��54      $��0,0,10,0q�55      56      �� 57      -   5        $��Отмена��57      #��[ 58      $	��120��59      $��40��5:      5;      5<      5=      5>      5?      