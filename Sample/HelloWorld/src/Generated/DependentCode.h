#ifndef __RTCOP_GENERATED_DEPENDENTCODE__
#define __RTCOP_GENERATED_DEPENDENTCODE__

namespace baselayer { class Hello; } 
namespace English { class Hello; } 
namespace Japanese { class Hello; } 

namespace RTCOP {
namespace Generated {

class BaseLayer;
class English;
class Japanese;

namespace DependentCode {

volatile void* GetLayerdObjectFinalizer(::English::Hello* obj);
volatile void* GetLayerdObjectFinalizer(::Japanese::Hello* obj);

namespace baselayer { namespace Hello { 
volatile void** GetVirtualFunctionTable(BaseLayer* layer);
volatile void** GetVirtualFunctionTable(English* layer);
volatile void** GetVirtualFunctionTable(Japanese* layer);

void ExecuteProceed_Print(void* layerdObject, volatile void* proceed);

}}


} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED_DEPENDENTCODE__
