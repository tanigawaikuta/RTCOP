#include "./Generated/DependentCode.h"
#include "./Generated/Layer1.h"
#include "./Generated/Layer2.h"

namespace RTCOP {
namespace Generated {
namespace DependentCode {

namespace baselayer { namespace A { 
void ExecuteProceed_m1(void* layerdObject, volatile void* proceed)
{
	__asm
	{
		mov ecx, dword ptr[layerdObject]
		mov eax, dword ptr[proceed]
		call eax
	}
}

 }  } 

namespace baselayer { namespace B { 
void ExecuteProceed_m2(void* layerdObject, volatile void* proceed)
{
	__asm
	{
		mov ecx, dword ptr[layerdObject]
		mov eax, dword ptr[proceed]
		call eax
	}
}

 }  } 

volatile void* GetLayerdObjectFinalizer(::Layer1::A* obj)
{
	void* vfp = 0;
	typedef ::Layer1::A CLayer1A;
	__asm
	{
		mov eax, CLayer1A::_RTCOP_FinalizePartialClass
		mov vfp, eax
	}
	return vfp;
}

volatile void* GetLayerdObjectFinalizer(::Layer2::A* obj)
{
	void* vfp = 0;
	typedef ::Layer2::A CLayer2A;
	__asm
	{
		mov eax, CLayer2A::_RTCOP_FinalizePartialClass
		mov vfp, eax
	}
	return vfp;
}

volatile void* GetLayerdObjectFinalizer(::Layer2::B* obj)
{
	void* vfp = 0;
	typedef ::Layer2::B CLayer2B;
	__asm
	{
		mov eax, CLayer2B::_RTCOP_FinalizePartialClass
		mov vfp, eax
	}
	return vfp;
}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
