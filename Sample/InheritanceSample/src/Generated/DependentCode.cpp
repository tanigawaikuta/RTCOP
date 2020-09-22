#include "./Generated/DependentCode.h"

namespace RTCOP {
namespace Generated {
namespace DependentCode {

namespace baselayer { namespace SuperA { 
void ExecuteProceed_m1(void* layerdObject, volatile void* proceed)
{
	void(*pProceed)(void*) = (void(*)(void*))proceed;
	pProceed(layerdObject);
}

}}

namespace baselayer { namespace SubA { 
void ExecuteProceed_m1(void* layerdObject, volatile void* proceed)
{
	void(*pProceed)(void*) = (void(*)(void*))proceed;
	pProceed(layerdObject);
}

}}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
