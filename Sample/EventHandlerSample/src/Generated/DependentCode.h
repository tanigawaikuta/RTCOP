#ifndef __RTCOP_GENERATED_DEPENDENTCODE__
#define __RTCOP_GENERATED_DEPENDENTCODE__

namespace baselayer { class Sample; } 
namespace Layer1 { class Sample; } 

namespace RTCOP {
namespace Generated {

class BaseLayer;
class Layer1;

namespace DependentCode {

volatile void* GetLayerdObjectFinalizer(::Layer1::Sample* obj);

namespace baselayer { namespace Sample { 
volatile void** GetVirtualFunctionTable(BaseLayer* layer);
volatile void** GetVirtualFunctionTable(Layer1* layer);


}}


} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}
#endif // !__RTCOP_GENERATED_DEPENDENTCODE__
