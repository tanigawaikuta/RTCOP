//================================================================================
// ExecuteLayerdObjectFinalizer.cpp
//
// 役割: レイヤードなオブジェクトのファイナライザを実行するための関数を提供する。
//       Windows x86のMinGW用の実装。
//================================================================================

#include "DependentCode/ExecuteLayerdObjectFinalizer.h"

#ifdef WIN32_MINGW

namespace RTCOP {
namespace DependentCode {

// LayerdObjectのFinalizer実行
void ExecuteLayerdObjectFinalizer(void* layerdObject, volatile void* finalizer)
{
	void(*pFinalizer)(void*) = (void(*)(void*))finalizer;
	pFinalizer(layerdObject);
}

} // namespace DependentCode {}
} // namespace RTCOP {}

#endif
