//================================================================================
// ExecuteProceed.cpp
//
// 役割: Proceedを実行するための関数を提供する。
//       Windows x64の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/ExecuteProceed.h"

#ifdef WIN64

namespace RTCOP {
namespace Generated {
namespace DependentCode {

namespace HelloClass {

// PrintのProceedを実行
void ExecuteProceed_Print(void* layerdObject, volatile void* proceed)
{
	void(*pProceed)(void*) = (void(*)(void*))proceed;
	pProceed(layerdObject);
}

// Print2のProceedを実行
void ExecuteProceed_Print2(void* layerdObject, volatile void* proceed, char arg)
{
	void(*pProceed)(void*, char) = (void(*)(void*, char))proceed;
	pProceed(layerdObject, arg);
}

} // namespace HelloClass {}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // WIN64
