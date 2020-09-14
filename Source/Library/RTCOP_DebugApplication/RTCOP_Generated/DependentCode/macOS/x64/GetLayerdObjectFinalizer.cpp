//================================================================================
// GetLayerdObjectFinalizer.cpp
//
// 役割: LayerdObjectのFinalizerを取得するための関数を提供する。
//       macOS x64の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/GetLayerdObjectFinalizer.h"
#include "RTCOP_Generated/JapaneseLayer.h"
#include "RTCOP_Generated/EnglishLayer.h"

#ifdef MACOS_X64

// Finalizer
asm(".rtcop_vfaddr1: .quad __ZN5RTCOP9Generated19JapaneseLayer_Hello27_RTCOP_FinalizePartialClassEv");
asm(".rtcop_vfaddr2: .quad __ZN5RTCOP9Generated18EnglishLayer_Hello27_RTCOP_FinalizePartialClassEv");

namespace RTCOP {
namespace Generated {
namespace DependentCode {

// Japaneseレイヤのファイナライザを取得
volatile void* GetLayerdObjectFinalizer(JapaneseLayer_Hello* obj)
{
	volatile void* vfp = 0;
	asm volatile("movq .rtcop_vfaddr1(%%rip), %0" : "=r"(vfp) : : "memory");

	return vfp;
}

// Englishレイヤのファイナライザを取得
volatile void* GetLayerdObjectFinalizer(EnglishLayer_Hello* obj)
{
	volatile void* vfp = 0;
	asm volatile("movq .rtcop_vfaddr2(%%rip), %0" : "=r"(vfp) : : "memory");

	return vfp;
}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // MACOS_X64
