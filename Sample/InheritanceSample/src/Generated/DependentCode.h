#ifndef __RTCOP_GENERATED_DEPENDENTCODE__
#define __RTCOP_GENERATED_DEPENDENTCODE__

namespace baselayer { class SuperA; } 
namespace Layer1 { class SuperA; } 
namespace baselayer { class SubA; } 
namespace Layer1 { class SubA; } 

namespace RTCOP {
namespace Generated {

class BaseLayer;
class Layer1;

namespace DependentCode {

volatile void* GetLayerdObjectFinalizer(::Layer1::SuperA* obj);
volatile void* GetLayerdObjectFinalizer(::Layer1::SubA* obj);

namespace baselayer { namespace SuperA { 
volatile void** GetVirtualFunctionTable(BaseLayer* layer);
volatile void** GetVirtualFunctionTable(Layer1* layer);

void ExecuteProceed_m1(void* layerdObject, volatile void* proceed);

}}

namespace baselayer { namespace SubA { 
volatile void** GetVirtualFunctionTable(BaseLayer* layer);
volatile void** GetVirtualFunctionTable(Layer1* layer);

void ExecuteProceed_m1(void* layerdObject, volatile void* proceed);

}}


} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED_DEPENDENTCODE__
