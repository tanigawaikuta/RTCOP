//================================================================================
// ExecuteLayerdObjectFinalizer.cpp
//
// 役割: レイヤードなオブジェクトのファイナライザを実行するための関数を提供する。
//       Linux x86の実装。
//================================================================================

#include "DependentCode/ExecuteLayerdObjectFinalizer.h"

#ifdef LINUX_X86

namespace RTCOP {
namespace DependentCode {

// LayerdObjectのFinalizer実行
void ExecuteLayerdObjectFinalizer(void* layerdObject, volatile void* finalizer)
{
	void(*pfinalizer)(void*) = (void(*)(void*))finalizer;
	pfinalizer(layerdObject);
}

} // namespace DependentCode {}
} // namespace RTCOP {}

#endif
