import 'dart:developer';

import 'package:curved_navigation_bar/curved_navigation_bar.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:flyingdarts_core/navigation/navigation_cubit.dart';
import 'package:flyingdarts_core/websocket/websocket_message.dart';
import 'package:flyingdarts_core/websocket/websocket_request.dart';
import 'package:flyingdarts_core/websocket/websocket_service.dart';
import 'package:flyingdarts_core/widgets/appbar/appbar_widget.dart';
import 'package:flyingdarts_shared/themes/theme.dart';
import 'package:provider/provider.dart';

class MyHomePage extends StatefulWidget {
  const MyHomePage({Key? key}) : super(key: key);

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  int _currentIndex = 0;
  late WebSocketService<MessageRequest> _webSocketService;
  final List<WebSocketMessage<MessageRequest>> _receivedMessages = [];
  @override
  void initState() {
    super.initState();
    if (!kIsWeb) {
      _webSocketService = WebSocketService<MessageRequest>();
      _webSocketService.messages.listen((WebSocketMessage<MessageRequest> message) {
        log(message.message?.Message ?? message.toString());
        setState(() {
          _receivedMessages.add(message);
        });
      });
    }
  }

  @override
  void dispose() {
    if (!kIsWeb) {
      _webSocketService.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    var cubit = context.watch<NavigationCubit>();
    var pages = cubit.state.pages;
    return Scaffold(
      appBar: const MyAppBar(),
      body: pages[_currentIndex],
      bottomNavigationBar: CurvedNavigationBar(
        backgroundColor: MyTheme.primaryColor,
        onTap: (int index) {
          setState(() {
            _currentIndex = index;
          });
        },
        items: <Widget>[
          const Icon(
            Icons.mic_none,
            size: 24,
            color: MyTheme.primaryColor,
          ),
          const Icon(
            Icons.keyboard_alt_outlined,
            size: 24,
            color: MyTheme.primaryColor,
          ),
          SvgPicture.asset(
            'assets/icons/dartboard_icon_outlined.svg',
            width: 24,
            height: 24,
            color: MyTheme.primaryColor,
          ),
        ],
      ),
    );
  }
}
