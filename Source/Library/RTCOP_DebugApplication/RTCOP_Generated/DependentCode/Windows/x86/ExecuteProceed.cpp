//================================================================================
// ExecuteProceed.cpp
//
// 役割: Proceedを実行するための関数を提供する。
//       Windows x86の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/ExecuteProceed.h"

#ifdef WIN32

namespace RTCOP {
namespace Generated {
namespace DependentCode {

namespace HelloClass {

// PrintのProceedを実行
void ExecuteProceed_Print(void* layerdObject, volatile void* proceed)
{
	__asm
	{
		mov         ecx, dword ptr[layerdObject]
		mov         eax, dword ptr[proceed]
		call        eax
	}
}

// Print2のProceedを実行
void ExecuteProceed_Print2(void* layerdObject, volatile void* proceed, char arg)
{
	__asm
	{
		push        dword ptr[arg]
		mov         ecx, dword ptr[layerdObject]
		mov         eax, dword ptr[proceed]
		call        eax
	}
}

} // namespace HelloClass {}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // WIN32
