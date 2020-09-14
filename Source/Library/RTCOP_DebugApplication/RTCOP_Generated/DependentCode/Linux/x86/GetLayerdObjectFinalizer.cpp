//================================================================================
// GetLayerdObjectFinalizer.cpp
//
// 役割: LayerdObjectのFinalizerを取得するための関数を提供する。
//       Linux x86の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/GetLayerdObjectFinalizer.h"
#include "RTCOP_Generated/JapaneseLayer.h"
#include "RTCOP_Generated/EnglishLayer.h"

#ifdef LINUX_X86

namespace RTCOP {
namespace Generated {
namespace DependentCode {

// Japaneseレイヤのファイナライザを取得
volatile void* GetLayerdObjectFinalizer(JapaneseLayer_Hello* obj)
{
	volatile void* vfp = 0;
	asm volatile("movl $_ZN5RTCOP9Generated19JapaneseLayer_Hello27_RTCOP_FinalizePartialClassEv, %0" : "=r"(vfp) : : "memory");

	return vfp;
}

// Englishレイヤのファイナライザを取得
volatile void* GetLayerdObjectFinalizer(EnglishLayer_Hello* obj)
{
	volatile void* vfp = 0;
	asm volatile("movl $_ZN5RTCOP9Generated18EnglishLayer_Hello27_RTCOP_FinalizePartialClassEv, %0" : "=r"(vfp) : : "memory");

	return vfp;
}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // LINUX_X86
