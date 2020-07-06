//================================================================================
// GetLayerdObjectFinalizer.cpp
//
// 役割: LayerdObjectのFinalizerを取得するための関数を提供する。
//       Windows x86のMinGW用の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/GetLayerdObjectFinalizer.h"
#include "RTCOP_Generated/JapaneseLayer.h"
#include "RTCOP_Generated/EnglishLayer.h"

#ifdef WIN32_MINGW

namespace RTCOP {
namespace Generated {
namespace DependentCode {

// Japaneseレイヤのファイナライザを取得
volatile void* GetLayerdObjectFinalizer(JapaneseLayer_Hello* obj)
{
	volatile void* vfp = 0;
	asm volatile("movl $__ZN5RTCOP9Generated19JapaneseLayer_Hello21_FinalizePartialClassEv, %0" : "=r"(vfp) : : "memory");

	return vfp;
}

// Englishレイヤのファイナライザを取得
volatile void* GetLayerdObjectFinalizer(EnglishLayer_Hello* obj)
{
	volatile void* vfp = 0;
	asm volatile("movl $__ZN5RTCOP9Generated18EnglishLayer_Hello21_FinalizePartialClassEv, %0" : "=r"(vfp) : : "memory");

	return vfp;
}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // WIN32_MINGW
