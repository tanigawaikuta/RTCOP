//================================================================================
// ExecuteProceed.cpp
//
// 役割: Proceedを実行するための関数を提供する。
//       Linux x86の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/ExecuteProceed.h"

#ifdef LINUX_X86

namespace RTCOP {
namespace Generated {
namespace DependentCode {

namespace HelloClass {

// PrintのProceedを実行
void ExecuteProceed_Print(void* layerdObject, volatile void* proceed)
{
	void(*pproceed)(void*) = (void(*)(void*))proceed;
	pproceed(layerdObject);
}

// Print2のProceedを実行
void ExecuteProceed_Print2(void* layerdObject, volatile void* proceed, char arg)
{
	void(*pproceed)(void*, char) = (void(*)(void*, char))proceed;
	pproceed(layerdObject, arg);
}

} // namespace HelloClass {}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // LINUX_X86
