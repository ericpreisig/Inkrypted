   M S B A M L   `   `   `  ���� D  @Inkrypted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Inkrypted.ViewInkrypted.View  !    Inkrypted.View.SettingsView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .localclr-namespace:Inkrypted.View     H DMahApps.Metro, Version=1.5.0.0, Culture=neutral, PublicKeyToken=null;controls,http://metro.mahapps.com/winfx/xaml/controls  .���   � ColumnDefinitions  �� $��20 ��� $
��Auto ��� $��20 � � RowDefinitions �� $
��Auto ��� $��10 ��� $
��Auto ��� $��10 ��� $
��Auto ��� $��10 �.+��� $��0��$��1��   AccentedSquareButtonStyle#��[  ���� *OpenAddUserCommand+.����  �� Orientation$ 
Horizontal=�.+��� $��20��$��20������  �� RelativeSource ��  �� AncestorType$ ButtonL� �� Path$ 
Foreground� J� OpacityMask !�  j� Stretch$
 Fill=�  appbar_settings !� Visual# [ �� $��4 0 0 0q�$��Center=�$��Ajouter un utilisateur���� $��2��$��1��#��[  ���� *ChangeFolderCommand+.���� $ 
Horizontal=�.+��� $��20��$��20������  �� $ ButtonL�$ 
Foreground� !� $
 Fill=�  appbar_folder# [ �� $��4 0 0 0q�$��Center=�$��Changer de dossier racine���� $��4��$��1��#��[  ���� *ChangeEncryptKeyCommand+.���� $ 
Horizontal=�.+��� $��20��$��20������  �� $ ButtonL�$ 
Foreground� !� $
 Fill=�  
appbar_key# [ �� $��4 0 0 0q�$��Center=�$$��Changer la clé de chiffrement��