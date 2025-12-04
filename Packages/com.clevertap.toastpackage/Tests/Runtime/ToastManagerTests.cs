using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using CleverTap.ToastPackage;

namespace CleverTap.ToastPackage.Tests
{
    public class ToastManagerTests
    {
        private ToastManager _toastManager;

        [SetUp]
        public void Setup()
        {
            _toastManager = ToastManager.Instance;
        }

        [Test]
        public void ToastManager_InstanceIsNotNull()
        {
            Assert.IsNotNull(_toastManager, "ToastManager instance should not be null");
        }

        [Test]
        public void ToastManager_IsSingleton()
        {
            var instance1 = ToastManager.Instance;
            var instance2 = ToastManager.Instance;
            Assert.AreEqual(instance1, instance2, "ToastManager should be a singleton");
        }

        [UnityTest]
        public IEnumerator ShowToast_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() => _toastManager.ShowToast("Test Message"),
                "ShowToast should not throw an exception");
            yield return null;
        }

        [Test]
        public void ShowToast_AcceptsNullMessage()
        {
            Assert.DoesNotThrow(() => _toastManager.ShowToast(null),
                "ShowToast should handle null messages");
        }

        [Test]
        public void ShowToast_AcceptsEmptyMessage()
        {
            Assert.DoesNotThrow(() => _toastManager.ShowToast(""),
                "ShowToast should handle empty messages");
        }

        [Test]
        public void ShowShortToast_CallsShowToast()
        {
            Assert.DoesNotThrow(() => _toastManager.ShowShortToast("Short Toast"),
                "ShowShortToast should not throw");
        }

        [Test]
        public void ShowLongToast_CallsShowToast()
        {
            Assert.DoesNotThrow(() => _toastManager.ShowLongToast("Long Toast"),
                "ShowLongToast should not throw");
        }

        [UnityTest]
        public IEnumerator ShowToast_WithDifferentDurations()
        {
            Assert.DoesNotThrow(() => _toastManager.ShowToast("Duration 0", 0));
            yield return new WaitForSeconds(0.1f);

            Assert.DoesNotThrow(() => _toastManager.ShowToast("Duration 1", 1));
            yield return new WaitForSeconds(0.1f);
        }

        [TearDown]
        public void Teardown()
        {
            if (_toastManager != null && _toastManager.gameObject != null)
            {
                Object.DestroyImmediate(_toastManager.gameObject);
            }
        }
    }
}