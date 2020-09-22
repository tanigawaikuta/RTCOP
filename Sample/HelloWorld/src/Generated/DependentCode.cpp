#include "./Generated/DependentCode.h"
#include "./Generated/English.h"
#include "./Generated/Japanese.h"

namespace RTCOP {
namespace Generated {
namespace DependentCode {

namespace baselayer { namespace Hello { 
void ExecuteProceed_Print(void* layerdObject, volatile void* proceed)
{
	__asm
	{
		mov ecx, dword ptr[layerdObject]
		mov eax, dword ptr[proceed]
		call eax
	}
}

}}

volatile void* GetLayerdObjectFinalizer(::English::Hello* obj)
{
	void* vfp = 0;
	typedef ::English::Hello CEnglishHello;
	__asm
	{
		mov eax, CEnglishHello::_RTCOP_FinalizePartialClass
		mov vfp, eax
	}
	return vfp;
}

volatile void* GetLayerdObjectFinalizer(::Japanese::Hello* obj)
{
	void* vfp = 0;
	typedef ::Japanese::Hello CJapaneseHello;
	__asm
	{
		mov eax, CJapaneseHello::_RTCOP_FinalizePartialClass
		mov vfp, eax
	}
	return vfp;
}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
