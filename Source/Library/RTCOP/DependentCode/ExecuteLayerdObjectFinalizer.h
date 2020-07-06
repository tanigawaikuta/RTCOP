//================================================================================
// ExecuteLayerdObjectFinalizer.h
//
// 役割: レイヤードなオブジェクトのファイナライザを実行するための関数を提供する。
//       この関数の実装は、プラットフォームによって異なる。
//================================================================================

#ifndef __RTCOP_DEPENDENTCODE_EXECUTELAYERDOBJECTFINALIZER__
#define __RTCOP_DEPENDENTCODE_EXECUTELAYERDOBJECTFINALIZER__

namespace RTCOP {
namespace DependentCode {

// LayerdObjectのFinalizer実行
void ExecuteLayerdObjectFinalizer(void* layerdObject, volatile void* finalizer);

} // namespace DependentCode {}
} // namespace RTCOP {}

#endif // !__RTCOP_DEPENDENTCODE_EXECUTELAYERDOBJECTFINALIZER__
