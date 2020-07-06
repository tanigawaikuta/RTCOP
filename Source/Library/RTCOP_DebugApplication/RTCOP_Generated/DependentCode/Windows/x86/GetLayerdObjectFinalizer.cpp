//================================================================================
// GetLayerdObjectFinalizer.cpp
//
// 役割: LayerdObjectのFinalizerを取得するための関数を提供する。
//       Windows x86の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/GetLayerdObjectFinalizer.h"
#include "RTCOP_Generated/JapaneseLayer.h"
#include "RTCOP_Generated/EnglishLayer.h"

#ifdef WIN32

namespace RTCOP {
namespace Generated {
namespace DependentCode {

// Japaneseレイヤのファイナライザを取得
volatile void* GetLayerdObjectFinalizer(JapaneseLayer_Hello* obj)
{
	void* vfp = 0;
	__asm
	{
		mov eax, JapaneseLayer_Hello::_FinalizePartialClass
		mov vfp, eax
	}
	return vfp;
}

// Englishレイヤのファイナライザを取得
volatile void* GetLayerdObjectFinalizer(EnglishLayer_Hello* obj)
{
	void* vfp = 0;
	__asm
	{
		mov eax, EnglishLayer_Hello::_FinalizePartialClass
		mov vfp, eax
	}
	return vfp;
}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // WIN32
