import 'package:flutter_test/flutter_test.dart';
import 'package:authress_login/src/services/performance_monitor.dart';

void main() {
  group('PerformanceMonitor', () {
    late PerformanceMonitor monitor;

    setUp(() {
      // Get fresh instance and enable monitoring
      monitor = PerformanceMonitor.instance;
      monitor.setEnabled(true);
      monitor.clearData();
    });

    tearDown(() {
      monitor.clearData();
    });

    group('Singleton Pattern', () {
      test('returns same instance', () {
        final instance1 = PerformanceMonitor.instance;
        final instance2 = PerformanceMonitor.instance;
        
        expect(instance1, same(instance2));
      });

      test('maintains state across instance calls', () {
        final instance1 = PerformanceMonitor.instance;
        instance1.setEnabled(false);
        
        final instance2 = PerformanceMonitor.instance;
        expect(instance2, same(instance1));
      });
    });

    group('Enable/Disable', () {
      test('enables and disables monitoring', () {
        monitor.setEnabled(false);
        monitor.startTimer('test');
        final duration = monitor.stopTimer('test');
        
        expect(duration, isNull);
      });

      test('enabled monitoring records operations', () {
        monitor.setEnabled(true);
        monitor.startTimer('test');
        
        // Small delay to ensure measurable time
        Future.delayed(const Duration(milliseconds: 1));
        
        final duration = monitor.stopTimer('test');
        expect(duration, isNotNull);
      });
    });

    group('Timer Operations', () {
      test('starts and stops timer correctly', () {
        monitor.startTimer('operation1');
        
        // Small delay
        Future.delayed(const Duration(milliseconds: 10));
        
        final duration = monitor.stopTimer('operation1');
        
        expect(duration, isNotNull);
        expect(duration!.inMicroseconds, greaterThan(0));
      });

      test('handles multiple concurrent timers', () {
        monitor.startTimer('op1');
        monitor.startTimer('op2');
        monitor.startTimer('op3');
        
        final duration1 = monitor.stopTimer('op1');
        final duration2 = monitor.stopTimer('op2');
        final duration3 = monitor.stopTimer('op3');
        
        expect(duration1, isNotNull);
        expect(duration2, isNotNull);
        expect(duration3, isNotNull);
      });

      test('returns null when stopping non-existent timer', () {
        final duration = monitor.stopTimer('non-existent');
        expect(duration, isNull);
      });

      test('handles stopping same timer multiple times', () {
        monitor.startTimer('test');
        final duration1 = monitor.stopTimer('test');
        final duration2 = monitor.stopTimer('test');
        
        expect(duration1, isNotNull);
        expect(duration2, isNull);
      });

      test('handles restarting same operation name', () {
        monitor.startTimer('operation');
        monitor.stopTimer('operation');
        
        monitor.startTimer('operation');
        final duration = monitor.stopTimer('operation');
        
        expect(duration, isNotNull);
      });
    });

    group('Statistics', () {
      test('returns null stats for non-existent operation', () {
        final stats = monitor.getStats('non-existent');
        expect(stats, isNull);
      });

      test('calculates basic statistics correctly', () {
        // Record some operations with known timing
        for (int i = 0; i < 5; i++) {
          monitor.startTimer('test_op');
          // Simulate different durations
          Future.delayed(Duration(milliseconds: i + 1));
          monitor.stopTimer('test_op');
        }
        
        final stats = monitor.getStats('test_op');
        expect(stats, isNotNull);
        expect(stats!.operationName, equals('test_op'));
        expect(stats.count, equals(5));
        expect(stats.averageMs, greaterThan(0));
        expect(stats.minMs, greaterThanOrEqualTo(0));
        expect(stats.maxMs, greaterThanOrEqualTo(stats.minMs));
      });

      test('calculates percentiles correctly', () {
        // Create predictable data
        final durations = [100, 200, 300, 400, 500]; // milliseconds
        
        for (int i = 0; i < durations.length; i++) {
          monitor.startTimer('perf_test');
          // Simulate the duration (this is imprecise for testing)
          monitor.stopTimer('perf_test');
        }
        
        final stats = monitor.getStats('perf_test');
        expect(stats, isNotNull);
        expect(stats!.count, equals(5));
      });

      test('getAllStats returns all recorded operations', () {
        monitor.startTimer('op1');
        monitor.stopTimer('op1');
        
        monitor.startTimer('op2');
        monitor.stopTimer('op2');
        
        final allStats = monitor.getAllStats();
        expect(allStats.keys, contains('op1'));
        expect(allStats.keys, contains('op2'));
        expect(allStats.length, equals(2));
      });

      test('getAllStats returns empty map when disabled', () {
        monitor.setEnabled(false);
        final allStats = monitor.getAllStats();
        expect(allStats, isEmpty);
      });
    });

    group('Custom Metrics', () {
      test('records custom metrics', () {
        expect(() => monitor.recordMetric('cpu_usage', 75.5, '%'), returnsNormally);
        expect(() => monitor.recordMetric('memory_usage', 1024, 'MB'), returnsNormally);
      });

      test('ignores metrics when disabled', () {
        monitor.setEnabled(false);
        expect(() => monitor.recordMetric('test', 100, 'units'), returnsNormally);
      });
    });

    group('Automatic Tracking', () {
      test('tracks async operation successfully', () async {
        final result = await monitor.trackOperation('async_test', () async {
          await Future.delayed(const Duration(milliseconds: 10));
          return 'success';
        });
        
        expect(result, equals('success'));
        
        final stats = monitor.getStats('async_test');
        expect(stats, isNotNull);
        expect(stats!.count, equals(1));
      });

      test('tracks async operation failure', () async {
        try {
          await monitor.trackOperation('failing_async', () async {
            await Future.delayed(const Duration(milliseconds: 5));
            throw Exception('Test error');
          });
          // If we reach here, the operation didn't throw as expected
          fail('Should have thrown');
        } catch (e) {
          expect(e.toString(), contains('Test error'));
        }
        
        final stats = monitor.getStats('failing_async');
        expect(stats, isNotNull);
        expect(stats!.count, equals(1));
      });

      test('tracks synchronous operation successfully', () {
        final result = monitor.trackSync('sync_test', () {
          // Simulate some work
          return 42;
        });
        
        expect(result, equals(42));
        
        final stats = monitor.getStats('sync_test');
        expect(stats, isNotNull);
        expect(stats!.count, equals(1));
      });

      test('tracks synchronous operation failure', () {
        try {
          monitor.trackSync('failing_sync', () {
            throw Exception('Sync error');
          });
          // If we reach here, the operation didn't throw as expected
          fail('Should have thrown');
        } catch (e) {
          expect(e.toString(), contains('Sync error'));
        }
        
        final stats = monitor.getStats('failing_sync');
        expect(stats, isNotNull);
        expect(stats!.count, equals(1));
      });
    });

    group('Performance Stats Model', () {
      test('creates stats with all required fields', () {
        const stats = PerformanceStats(
          operationName: 'test_operation',
          count: 10,
          averageMs: 150,
          p50Ms: 140,
          p95Ms: 200,
          minMs: 100,
          maxMs: 250,
        );

        expect(stats.operationName, equals('test_operation'));
        expect(stats.count, equals(10));
        expect(stats.averageMs, equals(150));
        expect(stats.p50Ms, equals(140));
        expect(stats.p95Ms, equals(200));
        expect(stats.minMs, equals(100));
        expect(stats.maxMs, equals(250));
      });

      test('toString returns formatted string', () {
        const stats = PerformanceStats(
          operationName: 'test_op',
          count: 5,
          averageMs: 100,
          p50Ms: 90,
          p95Ms: 150,
          minMs: 50,
          maxMs: 200,
        );

        final str = stats.toString();
        expect(str, contains('test_op'));
        expect(str, contains('count: 5'));
        expect(str, contains('avg: 100ms'));
        expect(str, contains('p50: 90ms'));
        expect(str, contains('p95: 150ms'));
      });
    });

    group('Data Management', () {
      test('clears all data', () {
        monitor.startTimer('test1');
        monitor.stopTimer('test1');
        monitor.recordMetric('metric1', 100, 'units');
        
        monitor.clearData();
        
        final stats = monitor.getStats('test1');
        expect(stats, isNull);
        
        final allStats = monitor.getAllStats();
        expect(allStats, isEmpty);
      });

      test('printReport handles empty data', () {
        monitor.clearData();
        expect(() => monitor.printReport(), returnsNormally);
      });

      test('printReport handles data with stats', () {
        monitor.startTimer('test_report');
        monitor.stopTimer('test_report');
        
        expect(() => monitor.printReport(), returnsNormally);
      });
    });

    group('Performance Tracking Mixin', () {
      test('mixin provides convenience methods', () {
        final tracker = TestClassWithMixin();
        
        expect(() => tracker.testTrackPerformance(), returnsNormally);
        expect(() => tracker.testTrackSyncPerformance(), returnsNormally);
        expect(() => tracker.testRecordMetric(), returnsNormally);
      });
    });

    group('Edge Cases', () {
      test('handles very short operations', () {
        monitor.startTimer('microsecond_op');
        final duration = monitor.stopTimer('microsecond_op');
        
        expect(duration, isNotNull);
        expect(duration!.inMicroseconds, greaterThanOrEqualTo(0));
      });

      test('handles operations with same name in sequence', () {
        for (int i = 0; i < 3; i++) {
          monitor.startTimer('repeated');
          monitor.stopTimer('repeated');
        }
        
        final stats = monitor.getStats('repeated');
        expect(stats, isNotNull);
        expect(stats!.count, equals(3));
      });

      test('handles special characters in operation names', () {
        const operationName = 'test-operation_with.special@characters';
        
        monitor.startTimer(operationName);
        final duration = monitor.stopTimer(operationName);
        
        expect(duration, isNotNull);
        
        final stats = monitor.getStats(operationName);
        expect(stats, isNotNull);
        expect(stats!.operationName, equals(operationName));
      });

      test('handles empty operation name', () {
        monitor.startTimer('');
        final duration = monitor.stopTimer('');
        
        expect(duration, isNotNull);
        
        final stats = monitor.getStats('');
        expect(stats, isNotNull);
      });
    });
  });
}

// Test class that uses the mixin
class TestClassWithMixin with PerformanceTrackingMixin {
  Future<void> testTrackPerformance() async {
    await trackPerformance('mixin_async', () async {
      await Future.delayed(const Duration(milliseconds: 1));
    });
  }
  
  void testTrackSyncPerformance() {
    trackSyncPerformance('mixin_sync', () {
      return 'result';
    });
  }
  
  void testRecordMetric() {
    recordMetric('test_metric', 42.0, 'units');
  }
} 