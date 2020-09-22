#ifndef __RTCOP_GENERATED_DEPENDENTCODE__
#define __RTCOP_GENERATED_DEPENDENTCODE__

class Hello;
namespace Japanese { class Hello; } 

namespace RTCOP {
namespace Generated {

class BaseLayer;
class Japanese;

namespace DependentCode {

volatile void* GetLayerdObjectFinalizer(::Japanese::Hello* obj);

namespace Hello { 
volatile void** GetVirtualFunctionTable(BaseLayer* layer);
volatile void** GetVirtualFunctionTable(Japanese* layer);

void ExecuteProceed_Print(void* layerdObject, volatile void* proceed);

}


} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED_DEPENDENTCODE__
