//================================================================================
// GetLayerdObjectFinalizer.h
//
// 役割: LayerdObjectのFinalizerを取得するための関数を提供する。
//       この関数の実装は、プラットフォームによって異なる。
//================================================================================

#ifndef __RTCOP_GENERATED_DEPENDENTCODE_GETLAYERDOBJECTFINALIZER__
#define __RTCOP_GENERATED_DEPENDENTCODE_GETLAYERDOBJECTFINALIZER__

namespace RTCOP {
namespace Generated {

class JapaneseLayer_Hello;
class EnglishLayer_Hello;

namespace DependentCode {

// Finalizerの取得
volatile void* GetLayerdObjectFinalizer(JapaneseLayer_Hello* obj);
volatile void* GetLayerdObjectFinalizer(EnglishLayer_Hello* obj);

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // !__RTCOP_GENERATED_DEPENDENTCODE_EXECUTEPROCEED__
