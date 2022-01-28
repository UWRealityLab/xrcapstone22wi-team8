import 'package:flutter/material.dart';
import 'package:firebase_core/firebase_core.dart';
import 'firebase_options.dart';
import 'package:file_picker/file_picker.dart';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_storage/firebase_storage.dart' as firebase_storage;
import 'package:flutter/services.dart';

FirebaseFirestore firestore = FirebaseFirestore.instance;
firebase_storage.FirebaseStorage storage =
    firebase_storage.FirebaseStorage.instance;

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Outside Boardless',
      theme: ThemeData(
        // This is the theme of your application.
        //
        // Try running your application with "flutter run". You'll see the
        // application has a blue toolbar. Then, without quitting the app, try
        // changing the primarySwatch below to Colors.green and then invoke
        // "hot reload" (press "r" in the console where you ran "flutter run",
        // or simply save your changes to "hot reload" in a Flutter IDE).
        // Notice that the counter didn't reset back to zero; the application
        // is not restarted.
        primarySwatch: Colors.deepPurple,
      ),
      home: const MyHomePage(title: 'Boardless Files (room: test)'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({Key? key, required this.title}) : super(key: key);

  // This widget is the home page of your application. It is stateful, meaning
  // that it has a State object (defined below) that contains fields that affect
  // how it looks.

  // This class is the configuration for the state. It holds the values (in this
  // case the title) provided by the parent (in this case the App widget) and
  // used by the build method of the State. Fields in a Widget subclass are
  // always marked "final".

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  static const String room = "test";

  final Stream<QuerySnapshot> _usersStream =
      FirebaseFirestore.instance.collection(room).snapshots();

  void _selectFile() async {
    FilePickerResult? result = await FilePicker.platform.pickFiles();

    if (result != null) {
      PlatformFile rawFile = result.files.single;
      String fileName = rawFile.name;
      if (rawFile.extension == "txt") {
        String contents = String.fromCharCodes(rawFile.bytes!);
        firestore
            .collection(room)
            .add({'name': fileName, 'content': contents})
            .then((value) => print("Text added"))
            .catchError((error) => print("Failed to add text: $error"));
      } else {
        storage.ref().child(room).child(fileName).putData(rawFile.bytes!);
        firestore
            .collection(room)
            .add({'name': fileName, 'ref': fileName})
            .then((value) => print("File added"))
            .catchError((error) => print("Failed to add file: $error"));
      }
    } else {
      // User canceled the picker
      print("No file selected");
    }
  }

  @override
  Widget build(BuildContext context) {
    // This method is rerun every time setState is called, for instance as done
    // by the _incrementCounter method above.
    //
    // The Flutter framework has been optimized to make rerunning build methods
    // fast, so that you can just rebuild anything that needs updating rather
    // than having to individually change instances of widgets.
    return Scaffold(
      appBar: AppBar(
        // Here we take the value from the MyHomePage object that was created by
        // the App.build method, and use it to set our appbar title.
        title: Text(widget.title),
      ),
      body: Center(
        // Center is a layout widget. It takes a single child and positions it
        // in the middle of the parent.
        child: StreamBuilder<QuerySnapshot>(
          stream: _usersStream,
          builder:
              (BuildContext context, AsyncSnapshot<QuerySnapshot> snapshot) {
            if (snapshot.hasError) {
              return const Text('Something went wrong');
            }

            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Text("Loading");
            }

            if (snapshot.data!.docs.isEmpty) {
              return const Text("No documents");
            }

            return ListView.separated(
                itemBuilder: (context, index) {
                  DocumentSnapshot document = snapshot.data!.docs[index];
                  Map<String, dynamic> data =
                      document.data()! as Map<String, dynamic>;
                  if (data.containsKey("ref")) {
                    return ListTile(
                      title: Text(data['name']),
                      subtitle: Text(data['ref']),
                    );
                  } else {
                    return ListTile(
                      title: Text(data['content']),
                      subtitle: Text(data['name'] ?? "Plain Text"),
                      trailing: ElevatedButton(
                          onPressed: () {
                            Clipboard.setData(
                                ClipboardData(text: data['content']));
                          },
                          child: const Text("Copy")),
                    );
                  }
                },
                separatorBuilder: (context, index) => const Divider(),
                itemCount: snapshot.data!.docs.length);
          },
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _selectFile,
        tooltip: 'Add File',
        child: const Icon(Icons.add),
      ), // This trailing comma makes auto-formatting nicer for build methods.
    );
  }
}
