import 'package:flutter/material.dart';

class SwipeToDeleteBackground extends StatelessWidget {
  const SwipeToDeleteBackground({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.only(right: 20.0),
      alignment: Alignment.centerRight,
      color: Colors.red,
      child: const Text(
        'Delete',
        textAlign: TextAlign.right,
        style: TextStyle(color: Colors.white),
      ),
    );
  }
}
