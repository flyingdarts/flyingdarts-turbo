import 'dart:convert';
import 'dart:math';
import 'package:crypto/crypto.dart';
import 'package:flutter/foundation.dart';

/// Service for cryptographic operations including PKCE and anti-abuse hash generation
class CryptoService {
  static const int _defaultMaxIterations = 100000;

  /// Generate PKCE codes for OAuth security
  PKCECodes generatePKCECodes() {
    final random = Random.secure();
    final codeVerifierBytes = List.generate(32, (_) => random.nextInt(256));
    final codeVerifier = base64Url.encode(codeVerifierBytes).replaceAll('=', '');
    
    // Generate code challenge using SHA256
    final bytes = utf8.encode(codeVerifier);
    final digest = sha256.convert(bytes);
    final codeChallenge = base64Url.encode(digest.bytes).replaceAll('=', '');
    
    return PKCECodes(
      codeVerifier: codeVerifier,
      codeChallenge: codeChallenge,
      codeChallengeMethod: 'S256',
    );
  }

  /// Calculate anti-abuse hash with proof-of-work
  /// This matches the JavaScript implementation behavior
  Future<String> calculateAntiAbuseHash(
    Map<String, String?> props, {
    int maxIterations = _defaultMaxIterations,
  }) async {
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    final valueString = props.values
        .where((v) => v != null && v.isNotEmpty)
        .join('|');
    
    debugPrint('🔐 CryptoService: Calculating anti-abuse hash');
    debugPrint('   Props: $props');
    debugPrint('   Value string: "$valueString"');
    
    return _computeProofOfWork(timestamp, valueString, maxIterations);
  }

  /// Compute proof-of-work hash (can be moved to isolate for heavy computation)
  Future<String> _computeProofOfWork(
    int timestamp, 
    String valueString, 
    int maxIterations,
  ) async {
    // For expensive computations, consider using compute() to run in isolate
    if (maxIterations > 10000) {
      return compute(_computeProofOfWorkSync, _ProofOfWorkParams(
        timestamp: timestamp,
        valueString: valueString,
        maxIterations: maxIterations,
      ));
    }

    return _computeProofOfWorkSync(_ProofOfWorkParams(
      timestamp: timestamp,
      valueString: valueString,
      maxIterations: maxIterations,
    ));
  }

  /// Generate random state string for CSRF protection
  String generateRandomState([int length = 32]) {
    final random = Random.secure();
    return List.generate(
      length,
      (_) => random.nextInt(256).toRadixString(16).padLeft(2, '0'),
    ).join();
  }

  /// Generate random nonce
  String generateNonce([int length = 16]) {
    final random = Random.secure();
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    return List.generate(length, (_) => chars[random.nextInt(chars.length)]).join();
  }

  /// Validate JWT token format without parsing
  bool isValidJWTFormat(String token) {
    final parts = token.split('.');
    return parts.length == 3 && 
           parts.every((part) => part.isNotEmpty);
  }
}

/// Synchronous proof-of-work computation (for isolate execution)
String _computeProofOfWorkSync(_ProofOfWorkParams params) {
  int fineTuner = 0;
  String hash = '';
  
  // Find hash starting with "00"
  while (fineTuner < params.maxIterations) {
    fineTuner++;
    final input = '${params.timestamp};$fineTuner;${params.valueString}';
    final bytes = utf8.encode(input);
    final digest = sha256.convert(bytes);
    hash = base64Url.encode(digest.bytes).replaceAll('=', '');
    
    if (hash.startsWith('00')) {
      debugPrint('🔐 Anti-abuse hash found after $fineTuner iterations');
      break;
    }
  }
  
  if (fineTuner >= params.maxIterations) {
    debugPrint('⚠️ WARNING: Anti-abuse hash calculation reached max iterations');
  }
  
  final result = 'v2;${params.timestamp};$fineTuner;$hash';
  debugPrint('🔐 Final anti-abuse hash: $result');
  return result;
}

/// Parameters for proof-of-work computation
class _ProofOfWorkParams {
  final int timestamp;
  final String valueString;
  final int maxIterations;

  const _ProofOfWorkParams({
    required this.timestamp,
    required this.valueString,
    required this.maxIterations,
  });
}

/// PKCE codes container
class PKCECodes {
  final String codeVerifier;
  final String codeChallenge;
  final String codeChallengeMethod;

  const PKCECodes({
    required this.codeVerifier,
    required this.codeChallenge,
    required this.codeChallengeMethod,
  });

  Map<String, String> toMap() => {
    'codeVerifier': codeVerifier,
    'codeChallenge': codeChallenge,
    'codeChallengeMethod': codeChallengeMethod,
  };
} 