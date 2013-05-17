=== Structure of commands: ===
 - "XXX:YYYYYYYYY:ZZZZ"
 - X.. => Kód aplikace které se tento příkaz týká
 - Y.. => Objekt v aplikaci, kterého se příkaz týká
 - Z.. => Příkaz na tento objekt


=== General Commands ===
 
 - General Command Code: "000" // command that dont care about type of application

 == Objects ==
  * "User" => User of application

  = User =
  * "login(id)" => user with id logged in

=== Synchronized reading App: ===

  - App code: "501"

 == Objects ==
  * "App" => Whole application
  * "Pointer" => Pointer where is the text readed
  * "Text" => Readed text
 
  = App =
  * "Start()" => Start of application
  * "Close()" => Application closed

  = Pointer =
  * "Possition(x,y)" => Position of pointer
  * "Hide()" => Hide a pointer
  * "Show()" => Show a pointer

  = Text =
  * "Highlight(RGBA(R, G, B, A); startingRange; endingRange)" => Highlight text in specified range



  /* Notes */
  url for RTF file on a web: http://38511.w11.wedos.ws/domains/kavoj.cz/testRTF.rtf

  /* Ikony */
  http://icons8.com/license/
  http://icons8.com/download-huge-windows8-set/
