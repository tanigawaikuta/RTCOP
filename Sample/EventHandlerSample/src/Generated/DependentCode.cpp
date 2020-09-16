#include "./Generated/DependentCode.h"
#include "./Generated/Layer1.h"

namespace RTCOP {
namespace Generated {
namespace DependentCode {

namespace baselayer { namespace Sample { 

 }  } 

volatile void* GetLayerdObjectFinalizer(::Layer1::Sample* obj)
{
	void* vfp = 0;
	typedef ::Layer1::Sample CLayer1Sample;
	__asm
	{
		mov eax, CLayer1Sample::_RTCOP_FinalizePartialClass
		mov vfp, eax
	}
	return vfp;
}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
