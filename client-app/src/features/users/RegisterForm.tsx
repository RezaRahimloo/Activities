import { ErrorMessage, Form, Formik } from "formik";
import MyTextInput from "../../app/common/form/MyTextInput";
import { Button, Header, Label } from "semantic-ui-react";
import { useStore } from "../../app/strores/store";
import { observer } from "mobx-react-lite";
import * as Yup from 'yup'
import ValidationErrors from "../errors/ValidationErrors";

export default observer(function RegisterForm() {
    const { userStore } = useStore();

    return (
        <Formik
            initialValues={{ displayName: '', email: '', password: '', error: null }}
            validationSchema={Yup.object({
                displayName: Yup.string().required(),
                email: Yup.string().required().email(),
                password: Yup.string().required()
            })}
            onSubmit={(values, { setErrors }) =>
                userStore.login(values)
                .catch(error => setErrors({ error }))}>
            {({ handleSubmit, isSubmitting, errors, isValid, dirty }) => (
                <Form className="ui form error" onSubmit={handleSubmit} autoComplete="off">
                    <Header as={"h2"} content="Sign up to Reactivities" color="teal" textAlign="center"/>
                    <MyTextInput name="email" placeholder="Email" />
                    <MyTextInput name="displayName" placeholder="displayName" />
                    <MyTextInput type="password" name="password" placeholder="Password" />
                    <ErrorMessage name="error" render={() => (
                        <ValidationErrors errors={errors.error}/>
                    )}/>
                    <Button disabled={!isValid || !dirty || isSubmitting} loading={isSubmitting} positive content="Register" type="submit" fluid />
                </Form>
            )}
        </Formik>
    )
});