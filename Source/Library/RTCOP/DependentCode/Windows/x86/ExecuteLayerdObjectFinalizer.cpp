//================================================================================
// ExecuteLayerdObjectFinalizer.cpp
//
// 役割: レイヤードなオブジェクトのファイナライザを実行するための関数を提供する。
//       Windows x86の実装。
//================================================================================

#include "DependentCode/ExecuteLayerdObjectFinalizer.h"

#ifdef WIN32

namespace RTCOP {
namespace DependentCode {

// LayerdObjectのFinalizer実行
void ExecuteLayerdObjectFinalizer(void* layerdObject, volatile void* finalizer)
{
	__asm
	{
		mov         ecx, dword ptr[layerdObject]
		mov         eax, dword ptr[finalizer]
		call        eax
	}
}

} // namespace DependentCode {}
} // namespace RTCOP {}

#endif
