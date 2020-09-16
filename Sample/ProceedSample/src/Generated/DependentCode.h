#ifndef __RTCOP_GENERATED_DEPENDENTCODE__
#define __RTCOP_GENERATED_DEPENDENTCODE__

namespace baselayer { class A; } 
namespace Layer1 { class A; } 
namespace Layer2 { class A; } 
namespace baselayer { class B; } 
namespace Layer2 { class B; } 

namespace RTCOP {
namespace Generated {

class BaseLayer;
class Layer1;
class Layer2;

namespace DependentCode {

volatile void* GetLayerdObjectFinalizer(::Layer1::A* obj);
volatile void* GetLayerdObjectFinalizer(::Layer2::A* obj);
volatile void* GetLayerdObjectFinalizer(::Layer2::B* obj);

namespace baselayer { namespace A { 
volatile void** GetVirtualFunctionTable(BaseLayer* layer);
volatile void** GetVirtualFunctionTable(Layer1* layer);
volatile void** GetVirtualFunctionTable(Layer2* layer);

void ExecuteProceed_m1(void* layerdObject, volatile void* proceed);

 }  } 
namespace baselayer { namespace B { 
volatile void** GetVirtualFunctionTable(BaseLayer* layer);
volatile void** GetVirtualFunctionTable(Layer2* layer);

void ExecuteProceed_m2(void* layerdObject, volatile void* proceed);

 }  } 

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED_DEPENDENTCODE__
