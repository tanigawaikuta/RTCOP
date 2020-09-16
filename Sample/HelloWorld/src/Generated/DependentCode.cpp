#include "./Generated/DependentCode.h"

namespace RTCOP {
namespace Generated {
namespace DependentCode {

namespace baselayer { namespace Hello { 
void ExecuteProceed_Print(void* layerdObject, volatile void* proceed)
{
	void(*pProceed)(void*) = (void(*)(void*))proceed;
	pProceed(layerdObject);
}

 }  } 
} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
