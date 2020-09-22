#ifndef __RTCOP_GENERATED_BASELAYER__
#define __RTCOP_GENERATED_BASELAYER__

#include "RTCOP/Core/Layer.h"
#include "RTCOP/Core/LayerdObject.h"



namespace RTCOP {
namespace Generated {

class BaseLayer : public Core::Layer
{
public:
	static BaseLayer* GetInstance();
	explicit BaseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	virtual ~BaseLayer() override;
protected:
	virtual void* InitializeLayerdObject(void* obj, int classID) override;
};

} // namespace Generated {}
} // namespace RTCOP {}



#endif //!__RTCOP_GENERATED_JAPANESELAYER__
