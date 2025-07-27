import 'package:curved_navigation_bar/curved_navigation_bar.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:ui/themes/theme.dart';

/// Shell widget for the home page with curved navigation bar
class HomeShell extends StatefulWidget {
  final StatefulNavigationShell navigationShell;

  const HomeShell({
    super.key,
    required this.navigationShell,
  });

  @override
  State<HomeShell> createState() => _HomeShellState();
}

class _HomeShellState extends State<HomeShell> {
  int _currentIndex = 0;

  @override
  void initState() {
    super.initState();
    _currentIndex = widget.navigationShell.currentIndex;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: widget.navigationShell,
      bottomNavigationBar: CurvedNavigationBar(
        backgroundColor: MyTheme.primaryColor,
        color: Colors.white,
        height: 75,
        animationDuration: Duration(milliseconds: 600),
        animationCurve: Curves.easeOutCubic,
        index: _currentIndex,
        items: const [
          Icon(
            Icons.dashboard,
            size: 30,
            color: MyTheme.primaryColor,
          ),
          Icon(
            Icons.mic_sharp,
            size: 30,
            color: MyTheme.primaryColor,
          ),
          Icon(
            Icons.keyboard,
            size: 30,
            color: MyTheme.primaryColor,
          ),
        ],
        onTap: (index) {
          setState(() {
            _currentIndex = index;
          });
          widget.navigationShell.goBranch(index);
        },
      ),
    );
  }
}
