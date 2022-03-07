import 'dart:typed_data';

import 'package:firebase_storage/firebase_storage.dart';
import 'package:flutter/widgets.dart';

class FirebaseImage extends StatefulWidget {
  const FirebaseImage(this.fileReference, {Key? key}) : super(key: key);

  final Reference fileReference;

  @override
  _FirebaseImageState createState() => _FirebaseImageState();
}

class _FirebaseImageState extends State<FirebaseImage> {
  Uint8List? imageBytes;
  String? errorText;
  bool failed = false;

  @override
  Widget build(BuildContext context) {
    if (imageBytes != null) {
      return Image.memory(imageBytes!);
    } else {
      if (errorText != null) {
        return Text("Could not load: ${errorText!}");
      } else {
        widget.fileReference
            .getData()
            .then((data) => setState(() {
                  imageBytes = data;
                }))
            .catchError((e) => setState(() {
                  errorText = e.toString();
                }));
        return const Text("Loading...");
      }
    }
  }
}
