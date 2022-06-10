# MessageDecompressor

This is a small tool that can be used to browse compressed Azure service bus messages, given a connection string and a queue name. There are commandline and GUI versions of the tool.

# Commandline 

The code for the commandline version of the tool is stored in `MessageDecompressor.Console`, and its usage is as follows:

`message_decompressor.exe <connection-string> <queue-name>`

This will list all decompressed messages into the console.

# GUI

The code for the commandline version of the tool is stored in `MessageDecompressor`. Just run the app and enter values in the text boxes for the connection string and queue name.