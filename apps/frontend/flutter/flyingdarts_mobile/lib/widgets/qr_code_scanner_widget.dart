import 'package:flutter/material.dart';
import 'package:ui/ui.dart';

/// A reusable widget for connecting to games via various methods
class ConnectToGameWidget extends StatelessWidget {
  const ConnectToGameWidget({super.key});

  @override
  Widget build(BuildContext context) {
    return Card(
      color: Colors.white.withOpacity(0.1),
      child: Padding(
        padding: EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(Icons.games, color: Colors.white70, size: 20),
                SizedBox(width: 8),
                Text(
                  'Connect to Game',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
            SizedBox(height: 12),
            Text(
              'Join a game by scanning a QR code or entering a game code manually.',
              style: TextStyle(
                color: Colors.white70,
                fontSize: 14,
              ),
            ),
            SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: () {
                      _showQrCodeScannerDialog(context);
                    },
                    icon: Icon(Icons.qr_code_scanner, color: Colors.white),
                    label: Text(
                      'Scan QR Code',
                      style: TextStyle(color: Colors.white),
                    ),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: MyTheme.secondaryColor,
                      foregroundColor: Colors.white,
                      padding: EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                  ),
                ),
                SizedBox(width: 12),
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: () {
                      _showGameCodeDialog(context);
                    },
                    icon: Icon(Icons.keyboard, color: Colors.white),
                    label: Text(
                      'Enter Code',
                      style: TextStyle(color: Colors.white),
                    ),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: MyTheme.primaryColor[700],
                      foregroundColor: Colors.white,
                      padding: EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  void _showQrCodeScannerDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          backgroundColor: MyTheme.primaryColor,
          title: Row(
            children: [
              Icon(Icons.qr_code_scanner, color: Colors.white),
              SizedBox(width: 8),
              Text(
                'Scan Game QR Code',
                style: TextStyle(color: Colors.white),
              ),
            ],
          ),
          content: Container(
            width: 300,
            height: 300,
            decoration: BoxDecoration(
              color: MyTheme.primaryColor[800],
              borderRadius: BorderRadius.circular(8),
              border: Border.all(color: MyTheme.secondaryColor.withOpacity(0.3)),
            ),
            child: Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.qr_code_scanner,
                    size: 64,
                    color: MyTheme.secondaryColor.withOpacity(0.5),
                  ),
                  SizedBox(height: 16),
                  Text(
                    'Camera access required',
                    style: TextStyle(
                      color: Colors.white.withOpacity(0.7),
                      fontSize: 16,
                    ),
                  ),
                  SizedBox(height: 8),
                  Text(
                    'Point camera at game QR code',
                    style: TextStyle(
                      color: Colors.white.withOpacity(0.5),
                      fontSize: 14,
                    ),
                  ),
                ],
              ),
            ),
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
              },
              child: Text(
                'Cancel',
                style: TextStyle(color: Colors.white70),
              ),
            ),
            ElevatedButton(
              onPressed: () {
                // TODO: Implement actual QR code scanning
                Navigator.of(context).pop();
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text('QR Code scanning coming soon!'),
                    backgroundColor: MyTheme.secondaryColor,
                  ),
                );
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: MyTheme.secondaryColor,
                foregroundColor: Colors.white,
              ),
              child: Text('Scan'),
            ),
          ],
        );
      },
    );
  }

  void _showGameCodeDialog(BuildContext context) {
    final TextEditingController codeController = TextEditingController();

    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          backgroundColor: MyTheme.primaryColor,
          title: Row(
            children: [
              Icon(Icons.keyboard, color: Colors.white),
              SizedBox(width: 8),
              Text(
                'Enter Game Code',
                style: TextStyle(color: Colors.white),
              ),
            ],
          ),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                'Enter the game code provided by the game host.',
                style: TextStyle(
                  color: Colors.white70,
                  fontSize: 14,
                ),
              ),
              SizedBox(height: 16),
              TextField(
                controller: codeController,
                style: TextStyle(color: Colors.white),
                decoration: InputDecoration(
                  hintText: 'Enter game code...',
                  hintStyle: TextStyle(color: Colors.white54),
                  filled: true,
                  fillColor: MyTheme.primaryColor[700],
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(8),
                    borderSide: BorderSide(color: MyTheme.secondaryColor.withOpacity(0.3)),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(8),
                    borderSide: BorderSide(color: MyTheme.secondaryColor),
                  ),
                ),
                textCapitalization: TextCapitalization.characters,
                textInputAction: TextInputAction.done,
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
              },
              child: Text(
                'Cancel',
                style: TextStyle(color: Colors.white70),
              ),
            ),
            ElevatedButton(
              onPressed: () {
                final gameCode = codeController.text.trim();
                if (gameCode.isNotEmpty) {
                  // TODO: Implement game connection logic
                  Navigator.of(context).pop();
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                      content: Text('Connecting to game: $gameCode'),
                      backgroundColor: MyTheme.secondaryColor,
                    ),
                  );
                }
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: MyTheme.secondaryColor,
                foregroundColor: Colors.white,
              ),
              child: Text('Connect'),
            ),
          ],
        );
      },
    );
  }
}
