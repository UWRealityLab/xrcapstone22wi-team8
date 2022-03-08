import 'package:boardless_outside/FirebaseImage.dart';
import 'package:boardless_outside/SwipeToDeleteBackground.dart';
import 'package:flutter/material.dart';
import 'package:firebase_core/firebase_core.dart';
import 'firebase_options.dart';
import 'package:file_picker/file_picker.dart';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_storage/firebase_storage.dart' as firebase_storage;
import 'package:flutter/services.dart';
import 'package:path/path.dart' as p;

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
      darkTheme: ThemeData(
        brightness: Brightness.dark,
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

  // https://developer.apple.com/library/archive/documentation/Miscellaneous/Reference/UTIRef/Articles/System-DeclaredUniformTypeIdentifiers.html
  static const Set<String> plainTextExtensions = {
    // common text files
    "txt", "md", "markdown",
    // Programming language files
    // Powering Boardless
    "cs", "dart", "java", "gradle",
    // Apple fans
    "swift", "m", "mm", "applescript", "scpt", "podspec",
    // programming language class
    "sml", "ml", "rkt", "rs",
    // C and system programming
    "c", "cpp", "cp", "c++", "cc", "cxx", "h", "hpp", "h++", "hxx", "s", "sv",
    // Web
    "html", "css", "js", "javascript", "ts", "tsx", "sql", "php", "go",
    // Scripting
    "sh", "pl", "pm", "py", "rb", "bat", "el",
    // DO NOT INCLUDE resource files as plain text
    // they can have better representation (e.g. folding list, table, etc.)
    // "xml", "json", "jsonp", "yaml", "yml", // config files
    // "tsv", "csv", // other tabular format
  };
  static const Set<String> plainTextNames = {
    // Common Programming-related plain text files
    ".gitignore", ".gitattributes", "README", "LICENSE",
    "gradle.properties", "Makefile", "Dockerfile", "Podfile",
  };
  static const Set<String> imageExtensions = {".jpg", ".jpeg", ".png"};

  final Stream<QuerySnapshot> _usersStream =
      firestore.collection(room).snapshots();

  void _selectFile() async {
    FilePickerResult? result =
        await FilePicker.platform.pickFiles(withData: true);

    if (result != null) {
      PlatformFile rawFile = result.files.single;
      String fileName = rawFile.name;
      if (plainTextExtensions.contains(rawFile.extension?.toLowerCase()) ||
          plainTextNames.contains(fileName)) {
        String contents = String.fromCharCodes(rawFile.bytes!);
        firestore.collection(room).add({'name': fileName, 'content': contents});
      } else {
        storage.ref().child(room).child(fileName).putData(rawFile.bytes!);
        firestore.collection(room).add({'name': fileName, 'ref': fileName});
      }
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
                    String ref = data['ref'];
                    if (imageExtensions
                        .contains(p.extension(ref).toLowerCase())) {
                      return Dismissible(
                          key: Key(document.id),
                          onDismissed: (direction) {
                            firestore
                                .collection(room)
                                .doc(document.id)
                                .delete();
                            storage.ref().child(room).child(ref).delete();
                          },
                          background: const SwipeToDeleteBackground(),
                          child: Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 16.0),
                            child: Row(
                              children: [
                                Text(data['name']),
                                const Spacer(),
                                FirebaseImage(
                                    storage.ref().child(room).child(ref))
                              ],
                            ),
                          ));
                    } else {
                      return Dismissible(
                          key: Key(document.id),
                          onDismissed: (direction) {
                            firestore
                                .collection(room)
                                .doc(document.id)
                                .delete();
                            storage.ref().child(room).child(ref).delete();
                          },
                          background: const SwipeToDeleteBackground(),
                          child: ListTile(
                              title: Text(data['name']), subtitle: Text(ref)));
                    }
                  } else {
                    return Dismissible(
                        key: Key(document.id),
                        onDismissed: (direction) {
                          firestore.collection(room).doc(document.id).delete();
                        },
                        background: const SwipeToDeleteBackground(),
                        child: ListTile(
                          title: Text(data['content']),
                          subtitle: Text(data['name'] ?? "Plain Text"),
                          trailing: ElevatedButton(
                              onPressed: () {
                                Clipboard.setData(
                                    ClipboardData(text: data['content']));
                              },
                              child: const Text("Copy")),
                        ));
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
