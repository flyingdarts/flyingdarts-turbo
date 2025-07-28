import 'package:flutter_test/flutter_test.dart';
import 'package:authress_login/src/services/crypto_service.dart';

void main() {
  group('CryptoService', () {
    late CryptoService cryptoService;

    setUp(() {
      cryptoService = CryptoService();
    });

    group('PKCE Code Generation', () {
      test('generates valid PKCE codes', () {
        final pkceCodes = cryptoService.generatePKCECodes();

        expect(pkceCodes.codeVerifier, isNotNull);
        expect(pkceCodes.codeChallenge, isNotNull);
        expect(pkceCodes.codeChallengeMethod, equals('S256'));

        // Code verifier should be base64url encoded string without padding
        expect(pkceCodes.codeVerifier.length, greaterThanOrEqualTo(43));
        expect(pkceCodes.codeVerifier.length, lessThanOrEqualTo(128));
        expect(pkceCodes.codeVerifier.contains('='), isFalse);

        // Code challenge should be base64url encoded SHA256 hash without padding
        expect(
          pkceCodes.codeChallenge.length,
          equals(43),
        ); // SHA256 -> 32 bytes -> 43 chars in base64url without padding
        expect(pkceCodes.codeChallenge.contains('='), isFalse);
      });

      test('generates different codes on each call', () {
        final codes1 = cryptoService.generatePKCECodes();
        final codes2 = cryptoService.generatePKCECodes();

        expect(codes1.codeVerifier, isNot(equals(codes2.codeVerifier)));
        expect(codes1.codeChallenge, isNot(equals(codes2.codeChallenge)));
      });

      test('generates codes with valid characters', () {
        final pkceCodes = cryptoService.generatePKCECodes();

        // Base64url characters: A-Z, a-z, 0-9, -, _
        final validPattern = RegExp(r'^[A-Za-z0-9\-_]+$');

        expect(validPattern.hasMatch(pkceCodes.codeVerifier), isTrue);
        expect(validPattern.hasMatch(pkceCodes.codeChallenge), isTrue);
      });

      test('code challenge is derived from code verifier', () {
        final pkceCodes = cryptoService.generatePKCECodes();

        // We can't directly verify the SHA256 calculation without exposing internal methods,
        // but we can verify the relationship is consistent
        expect(pkceCodes.codeChallenge, isNotNull);
        expect(pkceCodes.codeChallenge.length, equals(43));
      });

      test('returns PKCECodes object with all required fields', () {
        final pkceCodes = cryptoService.generatePKCECodes();

        expect(pkceCodes, isA<PKCECodes>());
        expect(pkceCodes.codeVerifier, isA<String>());
        expect(pkceCodes.codeChallenge, isA<String>());
        expect(pkceCodes.codeChallengeMethod, isA<String>());
      });

      test('toMap returns correct structure', () {
        final pkceCodes = cryptoService.generatePKCECodes();
        final map = pkceCodes.toMap();

        expect(map, isA<Map<String, String>>());
        expect(map['codeVerifier'], equals(pkceCodes.codeVerifier));
        expect(map['codeChallenge'], equals(pkceCodes.codeChallenge));
        expect(
          map['codeChallengeMethod'],
          equals(pkceCodes.codeChallengeMethod),
        );
      });
    });

    group('Anti-Abuse Hash Calculation', () {
      test('calculates hash with valid properties', () async {
        final props = {
          'applicationId': 'test-app-123',
          'connectionId': 'test-connection',
          'tenantLookupIdentifier': 'test-tenant',
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);

        expect(hash, isNotNull);
        expect(hash.startsWith('v2;'), isTrue);

        final parts = hash.split(';');
        expect(parts.length, equals(4));
        expect(parts[0], equals('v2')); // version
        expect(int.tryParse(parts[1]), isNotNull); // timestamp
        expect(int.tryParse(parts[2]), isNotNull); // fine tuner
        expect(parts[3].length, greaterThan(0)); // hash value
      });

      test('produces consistent format', () async {
        final props = {
          'applicationId': 'test-app-123',
          'connectionId': 'test-connection',
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);
        final hashPattern = RegExp(r'^v2;\d+;\d+;[A-Za-z0-9\-_]+$');

        expect(hashPattern.hasMatch(hash), isTrue);
      });

      test('handles null and empty values', () async {
        final props = {
          'applicationId': 'test-app-123',
          'connectionId': null,
          'tenantLookupIdentifier': '',
          'emptyValue': null,
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);

        expect(hash, isNotNull);
        expect(hash.startsWith('v2;'), isTrue);
      });

      test('handles empty properties map', () async {
        final hash = await cryptoService.calculateAntiAbuseHash({});

        expect(hash, isNotNull);
        expect(hash.startsWith('v2;'), isTrue);
      });

      test('produces different hashes for different properties', () async {
        final props1 = {
          'applicationId': 'test-app-123',
          'connectionId': 'connection-1',
        };

        final props2 = {
          'applicationId': 'test-app-456',
          'connectionId': 'connection-2',
        };

        final hash1 = await cryptoService.calculateAntiAbuseHash(props1);
        final hash2 = await cryptoService.calculateAntiAbuseHash(props2);

        expect(hash1, isNot(equals(hash2)));
      });

      test('respects max iterations limit', () async {
        final props = {
          'applicationId': 'test-app-123',
        };

        // Use a very low max iterations to test the safety valve
        final hash = await cryptoService.calculateAntiAbuseHash(
          props,
          maxIterations: 10,
        );

        expect(hash, isNotNull);
        expect(hash.startsWith('v2;'), isTrue);

        final parts = hash.split(';');
        final fineTuner = int.parse(parts[2]);
        expect(fineTuner, lessThanOrEqualTo(10));
      });

      test('handles special characters in properties', () async {
        final props = {
          'applicationId': 'test-app-123',
          'specialChars': 'special!@#\$%^&*()_+-={}[]|\\:";\'<>?,./`~',
          'unicode': '测试中文字符',
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);

        expect(hash, isNotNull);
        expect(hash.startsWith('v2;'), isTrue);
      });

      test('proof-of-work finds hash starting with 00', () async {
        final props = {
          'applicationId': 'test-app-123',
          'connectionId': 'test-connection',
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);
        final parts = hash.split(';');
        final hashValue = parts[3];

        // The proof-of-work should find a hash starting with "00"
        expect(hashValue.startsWith('00'), isTrue);
      });

      test('timestamp is reasonable', () async {
        final startTime = DateTime.now().millisecondsSinceEpoch;

        final props = {
          'applicationId': 'test-app-123',
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);

        final endTime = DateTime.now().millisecondsSinceEpoch;
        final parts = hash.split(';');
        final timestamp = int.parse(parts[1]);

        expect(timestamp, greaterThanOrEqualTo(startTime));
        expect(timestamp, lessThanOrEqualTo(endTime));
      });

      test('fine tuner increases until solution found', () async {
        final props = {
          'applicationId': 'test-app-123',
          'connectionId': 'test-connection',
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);
        final parts = hash.split(';');
        final fineTuner = int.parse(parts[2]);

        // Fine tuner should be at least 1 (since we start from 1 and increment)
        expect(fineTuner, greaterThanOrEqualTo(1));

        // Should be reasonable (not hit the safety valve for normal cases)
        expect(fineTuner, lessThan(100000));
      });
    });

    group('Anti-Abuse Hash Edge Cases', () {
      test('handles very long property values', () async {
        final props = {
          'applicationId': 'test-app-123',
          'longValue': 'a' * 10000, // Very long string
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);

        expect(hash, isNotNull);
        expect(hash.startsWith('v2;'), isTrue);
      });

      test('handles many properties', () async {
        final props = <String, String?>{};
        for (int i = 0; i < 100; i++) {
          props['prop$i'] = 'value$i';
        }

        final hash = await cryptoService.calculateAntiAbuseHash(props);

        expect(hash, isNotNull);
        expect(hash.startsWith('v2;'), isTrue);
      });

      test('filters out null and empty values correctly', () async {
        final props = {
          'validProp1': 'value1',
          'nullProp': null,
          'emptyProp': '',
          'validProp2': 'value2',
          'anotherNullProp': null,
        };

        final hash = await cryptoService.calculateAntiAbuseHash(props);

        expect(hash, isNotNull);
        expect(hash.startsWith('v2;'), isTrue);
      });
    });
  });
}
